# M# Language Reference

## Core Syntax Rules

1. **Every command must end with `;`**
   - This includes `means` assignments and `say`.
2. **Blocks use `{` and `}`**
3. **Text values must use double quotes**
4. **Standalone identifiers are invalid**
   - `banana;` is an error (`Undefined variable ...`).

## Supported Statements

### 1) Numeric assignment

MSharp
Number has value 123;
Pi has value 3.14;
Negative has value -9;

### 2) Text assignment

MSharp
Name means "MSharp";

### 3) Print

MSharp
say "Hello";
say name;
say x;

### 4) Method call (statement)

`MSharp
do add and use 1, 2;
do readString and use "Your name: ";

- Executes a built-in method.
- Return value is ignored in this form.

Structure: Do *variablename* and use *parameters*

### 5) Capture a method return value (`set`)

This implementation uses:

MSharp
set variableName to do methodName and use ...;

Examples:

MSharp
set total to do add and use 4, 5;
set op to do readString and use "Op: ";

- `set` can store either numbers or strings.

### 6) If / else blocks

MSharp
if do eq and use op, "+" {
    say "plus";
} else {
    say "not plus";
}

### 7) While loop

MSharp
running has value 1;
while running {
    say "Looping";
    running has value 0;
}

Condition truthiness:
- Number: `0` is false, non-zero is true
- String: `""` is false, non-empty is true

## Expressions

M# supports these expression forms:
- Number literal: `123`, `-5`, `3.14`
- String literal: `"hello"`
- Identifier: `x`
- Method call expression: `do add and use 1, 2`

Method call expressions are used in:
- `set`
- `if` conditions
- `while` conditions
- `say`

## Built-in Methods

### Input

- `readNumber(promptString)` -> number
- `readString(promptString)` -> string

Examples:

MSharp
set n to do readNumber and use "Enter a number: ";
set s to do readString and use "Enter text: ";

### Math

- `add(a, b)` -> number
- `sub(a, b)` -> number
- `mul(a, b)` -> number
- `div(a, b)` -> number
  - runtime error on division by zero

### String utilities

- `toString(x)` -> string
- `concat(a, b)` -> string

### Comparison (returns number truth values)

- `eq(a, b)` -> `1` if equal, else `0`
- `neq(a, b)` -> `1` if not equal, else `0`

This avoids adding a separate boolean type to variables.

## Grammar (Implemented)

Pseudo grammar for the starter interpreter:

Text
program        -> statement* EOF ;

statement      -> numberAssign
               | stringAssign
               | sayStmt
               | callStmt
               | setStmt
               | ifStmt
               | whileStmt
               | block ;

block          -> "{" statement* "}" ;

numberAssign   -> IDENT "has" "value" NUMBER ";" ;
stringAssign   -> IDENT "means" STRING ";" ;

sayStmt        -> "say" expression ";" ;
callStmt       -> callExpr ";" ;
setStmt        -> "set" IDENT "to" callExpr ";" ;

ifStmt         -> "if" expression block ("else" block)? ;
whileStmt      -> "while" expression block ;

callExpr       -> "do" IDENT "and" "use" (expression ("," expression)*)? ;

expression     -> NUMBER
               | STRING
               | IDENT
               | callExpr ;
```

---

## Error Examples

### Undefined variable / standalone identifier

```msharp
banana;
```

Error:
- `Undefined variable 'banana'. Standalone identifiers are not valid statements.`

### Missing semicolon

```msharp
name means "M#"
```

Error:
- `Missing semicolon ';' after text assignment.`

### Unexpected token

```msharp
set x do add and use 1, 2;
```

Error:
- `Expected keyword 'to' after variable name.`

### Unterminated string

```msharp
say "oops;
```

Error:
- `Unterminated string literal.`

---

## Full Example Programs

### hello.ms

```msharp
say "Hello from M#";
```

### variables.ms

```msharp
count has value 42;
price has value -3.5;
name means "MSharp";

say name;
say count;
say price;

set total to do add and use count, 8;
say total;

set line to do concat and use "Tool: ", name;
say line;
```

### calculator.ms

See `examples/calculator.ms` for the full looped calculator program.
It uses:
- `while`
- `if/else`
- `readNumber`
- `readString`
- `add/sub/mul/div`
- `eq/neq`
- `toString` and `concat`
