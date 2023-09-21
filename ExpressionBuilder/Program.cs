using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

public class ExpressionBuilder
{
    private static readonly List<User> userData = UserDataSeed();

    public static void Main(string[] args)
    {
        Console.WriteLine("Specify the property to filter");
        string propertyName = Console.ReadLine();
        Console.WriteLine("Value to search against: " + propertyName);
        string value = Console.ReadLine();

        var dn = GetDynamicQueryWithExpresionTrees(propertyName, value);
        var output = userData.Where(dn).ToList();

        foreach (var item in output)
        {
            Console.WriteLine("Filtered result:");
            Console.WriteLine("\t ID: " + item.ID);
            Console.WriteLine("\t First Name: " + item.FirstName);
            Console.WriteLine("\t Last Name: " + item.LastName);
        }
    }

    private static List<User> UserDataSeed()
    {
        return new List<User>
            {
                new User{ ID = 1, FirstName = "Kevin", LastName = "Garnett"},
                new User{ ID = 2, FirstName = "Gopriya", LastName = "Kota"},
                new User{ ID = 3, FirstName = "Kevin", LastName = "Durant"},
                 new User{ ID = 3, FirstName = "Pravallika", LastName = "T"},
                  new User{ ID = 4, FirstName = "Derrick", LastName = "Rose"},
                   new User{ ID = 5, FirstName = "Lebron", LastName = "James"},
                    new User{ ID = 6, FirstName = "JJ", LastName = "Bareia"}
            };
    }

    private static Func<User, bool> GetDynamicQueryWithFunc(string propName, object val)
    {
        Func<User, bool> exp = (t) => true;
        switch (propName)
        {
            case "ID":
                exp = d => d.ID == Convert.ToInt32(val);
                break;
            case "FirstName":
                exp = f => f.FirstName == Convert.ToString(val);
                break;
            case "LastName":
                exp = l => l.LastName == Convert.ToString(val);
                break;
            default:
                break;
        }
        return exp;
    }

    private static Func<User, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val)
    {
        //x =>
        var param = Expression.Parameter(typeof(User), "x");

        #region Convert to specific data type
        MemberExpression member = Expression.Property(param, propertyName);
        UnaryExpression valueExpression = GetValueExpression(propertyName, val, param);
        #endregion
        Expression body = Expression.Equal(member, valueExpression);
        var final = Expression.Lambda<Func<User, bool>>(body: body, parameters: param);
        return final.Compile();
    }

    private static UnaryExpression GetValueExpression(string propertyName, string val, ParameterExpression param)
    {
        var newmemeber = Expression.Property(param, propertyName);
        var propertyType = ((PropertyInfo)newmemeber.Member).PropertyType;
        var newconverter = TypeDescriptor.GetConverter(propertyType);

        if (!newconverter.CanConvertFrom(typeof(string)))
            throw new NotSupportedException();

        var propertyValue = newconverter.ConvertFromInvariantString(val);
        var constant = Expression.Constant(propertyValue);
        return Expression.Convert(constant, propertyType);
    }

}

public class User
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
