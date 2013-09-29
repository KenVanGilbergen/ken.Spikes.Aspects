ken.Spikes.Aspects
==================

Aspect testing
---------------
Many frameworks, many options, only one you can choose...

In this spike
-------------
* Castle
* Emit
* Fody
* PostSharp
* Snap
* MessageSink

More AOP
--------
* Linfu: https://github.com/philiplaureano/LinFu (compiler)
* NKalore: http://www.codeproject.com/Articles/12650/Using-AOP-in-C (compiler)
* Ayende:	http://ayende.com/blog/3474/logging-the-aop-way (implementing aspects options)

| Remoting Proxies | Easy to implement | Only on interfaces or MarshalByRefObjects |
| Deriving from ContextBoundObject | Easiest to implement | Bad performance |
| Compile-time subclassing | Easiest to understand | Interfaces or virtual methods only |
| Runtime subclassing | Very flexible | Interfaces or virtual methods only |
| Hooking into the profiler API | Extremely powerful | Not so good performance |
| Compile time IL-weaving | Good performance | Very hard to implement |
| Runtime IL-weaving | Very powerful | Very hard to implement |
