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

Compiled to JavaScript

```javascript
var x = 123
var y = 456
function foo(z) { return (() => { return (z) ? (() => { return z = x })() : (() => { return z = y })() })() }
var bar = foo
var baz = foo(0)
```