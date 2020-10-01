# CSharpTypePrinter

[![NuGet Badge](https://buildstats.info/nuget/CSharpTypePrinter)](https://www.nuget.org/packages/CSharpTypePrinter)[![fuget.org](https://www.fuget.org/packages/CSharpTypePrinter/badge.svg?v=1.0.0)](https://www.fuget.org/packages/CSharpTypePrinter/1.0.0/)

Targets **.NET Standard 2.0**

Prints a `System.Type` object as a valid C# literal, e.g. prints `typeof(A<X>.B<Y>.C)` as a `"A<X>.B<Y>.C"`

It happens that the code for this is the complex pile of details especially if we talk about nested generics.

So I wanted to automate it and get and the robust implementation. A similar code is used Today by three of my projects: [DryIoc](https://github.com/dadhi/DryIoc), [FastExpressionCompiler](https://github.com/dadhi/FastExpressionCompiler), [ImTools](https://github.com/dadhi/ImTools).

The library contains a single extension method: 

```cs
public static class TypePrinter 
{
    public static string ToCSharpCode(this Type type,
        bool stripNamespace = false, 
        Func<Type, string, string> printType = null, 
        bool printGenericTypeArgs = false) 
        { 
            //:-)
        }
}
```

The options include:

- `stripNamespace` self explanatory.
- `printType` function may configure the final result given the input type and the output string.
- `printGenericTypeArgs` will output open-generic type as `Blah<T>` instead of `Blah<>`. The default value was selected because my own primary use -case is the type inside the `typeof()` where `typeof(Blah<>)` is the valid and the `typeof(Blah<T>)` is not.


Happy coding! 
