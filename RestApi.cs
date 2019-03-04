using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

// RestApi for adding and removing money and users.  Handles JSON input and outputs.

public class RestApi
{
    private List<Person> people;

    // Initiating database.
    public RestApi(string database)
    {
        people = JsonConvert.DeserializeObject<List<Person>>(database);
    }
    
    // Retrieving data.
    public string Get(string url, string payload = null)
    {
        switch (url)
        {
            case "/users":
                if (payload != null)
                {
                    var temp = JsonConvert.DeserializeObject<Users>(payload);
                    if (temp.user != null)
                    {
                        return JsonConvert.SerializeObject(people.Where(x => x.name == temp.user));
                    }
                    return JsonConvert.SerializeObject(people.Where(x => x.name == temp.users[0]));
                }
                else
                {
                    return JsonConvert.SerializeObject(people.OrderBy(x => x.name));
                }
            default:
                return null;
        }
    }

    // Changing Data in database.  Includes adding new User, or initiating a transaction.
    public string Post(string url, string payload)
    {
        switch (url)
        {
            case "/add":
                var temp_user = JsonConvert.DeserializeObject<Users>(payload);
                people.Add(new Person { name = temp_user.user });
                return JsonConvert.SerializeObject(people.First(x => x.name == temp_user.user));
            case "/iou":
                var trans = JsonConvert.DeserializeObject<Transaction>(payload);
                var borrower = people.FirstOrDefault(x => x.name == trans.borrower);
                var lender = people.FirstOrDefault(x => x.name == trans.lender);
                Make_Transaction(trans.lender, trans.amount, borrower.owes);
                Make_Transaction(trans.borrower, trans.amount, lender.owed_by);
                borrower.Equalize();
                lender.Equalize();
                var traded_people = new List<Person>() { borrower, lender };
                return JsonConvert.SerializeObject(traded_people.OrderBy(x => x.name));
            default:
                return null;
        }
    }

    // Transaction performed.  Money is added if other party is already owed/owes list, or new entry is added.
    private void Make_Transaction(string person, float amount, IDictionary<string, float> dict_data)
    {
        if (dict_data.ContainsKey(person))
        {
            dict_data[person] += amount;
        }
        else
        {
            dict_data.Add(person, amount);
        }
    }

}

public class Person
{
    public string name { get; set; }

    public SortedDictionary<string, float> owes { get; set; }
    public SortedDictionary<string, float> owed_by { get; set; }
    public float balance { get { return Get_Balance(); } }


    public Person()
    {
        owed_by = new SortedDictionary<string, float>();
        owes = new SortedDictionary<string, float>();
    }

    // If same user owes money and is owed to this will unify those lists.
    public void Equalize()
    {
        var data = owed_by.Keys.Intersect(owes.Keys);
        List<string> s = new List<string>();
        foreach (string i in data)
        {
            s.Add(i);
        }

        if (s.Count() > 0)
        {

            foreach (string key in s)
            {
                if (owes[key] > owed_by[key])
                {
                    owes[key] -= owed_by[key];
                    owed_by.Remove(key);
                }
                else if (owes[key] < owed_by[key])
                {
                    owed_by[key] -= owes[key];
                    owes.Remove(key);
                }
                else
                {
                    owes.Remove(key);
                    owed_by.Remove(key);
                }
            }
        }
    }

    // Totals amount owed and who owes.
    private float Get_Balance()
    {
        float bal = 0;
        foreach (float val in owes.Values)
        {
            bal -= val;
        }
        foreach (float val in owed_by.Values)
        {
            bal += val;
        }

        return bal;
    }
}

// Contains the names of parties and amount of transaction.
public class Transaction
{
    public string lender { get; set; }
    public string borrower { get; set; }
    public float amount { get; set; }
}

// User lists for retrieving information.
public class Users
{
    public List<String> users { get; set; }
    public string user { get; set; }
}