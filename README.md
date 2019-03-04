# Rest API in C#
#### This is a rest API exercise from the C# track of exercism.io.  Goal was to make a cs script to track transactions between users through JSON I/O.

#### Implementation code is RestApi.cs
#### Example tests from exercism where Actual is what code outputs and Expected is the expected answer:
```
public void Lender_has_negative_balance()
    {
        var url = "/iou";
        var payload = "{\"lender\":\"Bob\",\"borrower\":\"Adam\",\"amount\":3.0}";
        var database = "[{\"name\":\"Adam\",\"owes\":{},\"owed_by\":{},\"balance\":0.0},{\"name\":\"Bob\",\"owes\":{\"Chuck\":3.0},\"owed_by\":{},\"balance\":-3.0},{\"name\":\"Chuck\",\"owes\":{},\"owed_by\":{\"Bob\":3.0},\"balance\":3.0}]";
        var sut = new RestApi(database);
        var actual = sut.Post(url, payload);
        var expected = "[{\"name\":\"Adam\",\"owes\":{\"Bob\":3.0},\"owed_by\":{},\"balance\":-3.0},{\"name\":\"Bob\",\"owes\":{\"Chuck\":3.0},\"owed_by\":{\"Adam\":3.0},\"balance\":0.0}]";
        Assert.Equal(expected, actual);
    }
```
