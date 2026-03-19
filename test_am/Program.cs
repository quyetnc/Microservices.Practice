using System;
using Microsoft.Extensions.DependencyInjection;
public class Program {
    public static void Main() {
        foreach(var m in typeof(ServiceCollectionExtensions).GetMethods()) {
            if(m.Name == "AddAutoMapper") Console.WriteLine(m);
        }
    }
}
