 using System;
 using System.Reflection;

/// <summary>
/// "Could not find type 'IMethodDecorator' or 'MethodDecoratorAttribute'"
/// It was intentional, yes (and a readme would certainly help in this regard).
/// The reason it was in the global namespace was to ensure that there could only be one attribute or interface to check for.
/// If it can be allowed in any namespace, the it would need to work for any and all attributes/interfaces with the appropriate name. 
/// And then you'd also need to issue a warning or something if there is an interface or attribute with the same name but different signature. 
/// So, it was simpler to just insist there be only one marker type, in the global namespace.
/// 
/// That said, I'm not against changing this, but I think any patch would need to consider what happens if there is more than one marker type in the assembly.
/// </summary>
public interface IMethodDecorator
{
    void OnEntry(MethodBase method);
    void OnExit(MethodBase method);
    void OnException(MethodBase method, Exception exception);
}

