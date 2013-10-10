# ken.Spikes.Aspects

## Aspect testing
Many frameworks, many options, only one you can choose...

## In this spike
* Castle
* Emit
* Fody
* PostSharp
* Snap
* MessageSink
* Linfu
* SheepAspect

### More AOP
* NKalore: http://www.codeproject.com/Articles/12650/Using-AOP-in-C (compiler)
* Ayende:	http://ayende.com/blog/3474/logging-the-aop-way (implementing aspects options)

### Techniques
* **Remoting Proxies**: easy to implement, only on interfaces or MarshalByRefObjects
* **Deriving from ContextBoundObject**: easiest to implement, bad performance
* **Compile-time subclassing**: easiest to understand, interfaces or virtual methods only
* **Runtime subclassing**: very flexible, interfaces or virtual methods only
* **Hooking into the profiler API**: extremely powerful, poor documentation
* **Compile time IL-weaving**: good performance, hard to implement
* **Runtime IL-weaving**: very powerful, very hard to implement
