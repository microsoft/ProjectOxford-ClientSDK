Contributing to Project Oxford Client Libraries
===============================================

So, you want to contribute on a client SDK for one of the Project Oxford APIs.
Here's what you need to know.

1.  Each SDK must include both a client library and a sample showing the API in
    action

2.  When building an SDK, it's important you support the most common development
    platforms and that we are consistent from project to project. We require you
    to build the following, using the associated coding guidelines, in priority
    order:

    -   .NET (Coding guidelines below)

    -   Android [(Coding guidelines for
        Java)](<http://source.android.com/source/code-style.html>)

    -   iOS Objective-C [(Coding guidelines for
        Cocoa)](<https://developer.apple.com/library/mac/documentation/Cocoa/Conceptual/CodingGuidelines/CodingGuidelines.html>)

    -   Optional: Client Javascript ([Coding guidelines for
        npm](<https://docs.npmjs.com/misc/coding-style>))

3.  Samples are important for illustrating how to actually call into the API.
    Samples should be as visual and reusable as possible.

    -   Do:

    -   Create a UI sample when possible.

    -   Make your sample user friendly. Expect that developers will want to try
        different mainline scenarios and key APIs.

    -   Create code that's easy for other developers to copy/paste into their
        own solutions

    -   Consider:

    -   Adding UI to allow devs to quickly copy/paste subscription keys, instead
        of updating them in the code or using a config file. The
        FaceAPI-WPF-Samples.sln provides an example.

    -   Don't:

    -   Leave your subscription key in the source of samples. You do not want
        your key to be abused by others.

4.  Always create a README.md for your top-level API root and for each platform.

    -   Use the existing README.md files as a reference for what information is
        useful here. In general, you want to describe the functionality of the
        API as well as specifics for how to build and run the project(s).

 

Coding Guidelines for C\#
-------------------------

The general rule we follow is "use Visual Studio defaults."

1.  Use [Allman style](<http://en.wikipedia.org/wiki/Indent_style#Allman_style>)
    braces, where each brace begins on a new line. A single-line statement block
    can go without braces, but the block must be properly indented on its own
    line, and it must not be nested in other statement blocks that use braces.

2.  Use four spaces of indentation (no tabs).

3.  Use \_camelCase for internal and private members and use readonly where
    possible. Prefix instance fields with \_, static fields with s\_, and thread
    static fields with t\_.

4.  Avoid this. unless absolutely necessary.

5.  Always specify the visibility, even if it's the default (that is, private
    string \_foo, not string \_foo).

6.  Namespace imports should be specified at the top of the file, outside of
    namespace declarations, and should be sorted alphabetically, with System.
    namespaces at the top and blank lines between different top level groups.

7.  Avoid more than one empty line at any time. For example, do not have two
    blank lines between members of a type.

8.  Avoid spurious free spaces. For example avoid if (someVar == 0)..., where
    the dots mark the spurious free spaces. Consider enabling "View White Space
    (Ctrl+E, S)" if using Visual Studio, to aid detection.

9.  Only use var when it's obvious what the variable type is (that is, var
    stream = new FileStream(...), not var stream = OpenStandardInput()).

10. Use language keywords instead of BCL types (that is, int, string, float
    instead of Int32, String, Single, and so on) for both type references as
    well as method calls (that is, int.Parse instead of Int32.Parse).

11. Use PascalCasing to name all constant local variables and fields. The only
    exception is for interop code where the constant value should exactly match
    the name and value of the code you are calling via interop.

 

 
