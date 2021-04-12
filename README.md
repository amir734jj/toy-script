# toy-script

Dynamically typed procedural language

Examples:

```scala
let x = 123
let y = 456
def foo(z) = {
    if (z) {
        z = x
    } else {
        z = y
    }
}
let bar = foo
let baz = foo(0)
```