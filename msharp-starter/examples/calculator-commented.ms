// ==========================================================
// M# CALCULATOR (HEAVILY COMMENTED / EXPLAIN-EVERYTHING MODE)
// ==========================================================
//
// GENERAL M# RULES USED HERE:
// - Every command ends with a semicolon ;
// - Blocks use curly braces { }
// - Strings are wrapped in double quotes "like this"
// - Variables can hold numbers or strings
// - "do ... and use ..." calls built-in methods
//
// The nested ifs look ugly, yes.
// That is because M# is tiny and we are not using fancy features yet.
// Welcome to early language design.
// ==========================================================


// "say" = print text or a variable to the console (like Console.WriteLine in C#)
// "M# Calculator" = string literal (text)
// ; = statement terminator (required)
say "M# Calculator";


// "running" = variable name (identifier)
// "has value" = numeric assignment syntax in M#
// "1" = number literal
//
// We use running as a loop flag:
// - 1 means "keep looping"
// - 0 means "stop looping"
running has value 1;


// "while" = loop keyword
// "running" = loop condition expression
//
// M# truthiness rules:
// - number 0 = false
// - any non-zero number = true
//
// So while running { ... } means:
// "Keep executing this block as long as running is not 0"
while running {

    // "set" = assign a variable from a method call result
    // "operation" = variable that will store what the user typed ("+", "-", "*", "/", or "exit")
    // "to" = required keyword in "set ... to ..."
    // "do readString and use ..." = call built-in method readString(prompt)
    //
    // "do" = starts a method call
    // "readString" = built-in method that prints a prompt and reads text input from the user
    // "and use" = required words before the argument list
    // "Operation (+ - * / or exit): " = prompt string shown to the user
    //
    // Result: operation becomes a STRING variable.
    set operation to do readString and use "Operation (+ - * / or exit): ";


    // "if" = conditional keyword
    // The condition is a method call:
    // do eq and use operation, "exit"
    //
    // "eq" = built-in equality comparison method
    // It returns:
    // - 1 if equal
    // - 0 if not equal
    //
    // Arguments passed to eq:
    // - operation (the variable the user typed)
    // - "exit" (a string literal)
    //
    // If the user typed exit, this condition is true (1).
    if do eq and use operation, "exit" {

        // Set running to 0 so the while loop stops on the next condition check.
        running has value 0;

        // Print goodbye message.
        say "Goodbye.";

    } else {
        // "else" = runs only if the previous if condition was false.
        //
        // If we're here, the user did NOT type "exit".
        // Now we check which math operator they typed.


        // Check if operation is NOT "+"
        // "neq" = built-in "not equal" comparison (returns 1 if not equal, 0 if equal)
        //
        // This style creates a chain:
        // if not +, check not -, check not *, check not / ...
        // If all are "not equal", then it's invalid.
        if do neq and use operation, "+" {

            // If operation is not "-", continue checking...
            if do neq and use operation, "-" {

                // If operation is not "*", continue checking...
                if do neq and use operation, "*" {

                    // If operation is not "/", then it is unknown/invalid.
                    if do neq and use operation, "/" {

                        // Print error for invalid operator input.
                        say "Unknown operation. Use +, -, *, /, or exit.";

                    } else {
                        // --------------------------------------------------
                        // DIVISION BRANCH (operation == "/")
                        // We only get here because:
                        // - It was not +, not -, not *
                        // - And it WAS "/" (because neq "/" was false)
                        // --------------------------------------------------


                        // Ask for first number.
                        // "readNumber" = built-in that shows a prompt and reads a numeric input.
                        // It keeps asking until a valid number is entered.
                        set first to do readNumber and use "First number: ";

                        // Ask for second number.
                        set second to do readNumber and use "Second number: ";


                        // Check if second == 0 (division by zero guard)
                        // eq(second, 0) returns 1 if second is exactly zero.
                        if do eq and use second, 0 {

                            // Print a safe error instead of crashing or throwing division-by-zero.
                            say "Cannot divide by zero.";

                        } else {
                            // -----------------------------
                            // Safe division and output
                            // -----------------------------

                            // "div" = built-in division method
                            // result = first / second
                            set result to do div and use first, second;

                            // "toString" = converts number -> string
                            // Needed because "concat" creates text output.
                            set resultText to do toString and use result;

                            // "concat" = joins two values as text
                            // Here it builds: "Result: " + resultText
                            set output to do concat and use "Result: ", resultText;

                            // Print final line (example: Result: 12.5)
                            say output;
                        }
                    }

                } else {
                    // --------------------------------------------------
                    // MULTIPLICATION BRANCH (operation == "*")
                    // --------------------------------------------------

                    // Read first and second numbers
                    set first to do readNumber and use "First number: ";
                    set second to do readNumber and use "Second number: ";

                    // "mul" = built-in multiplication
                    set result to do mul and use first, second;

                    // Convert number result to text
                    set resultText to do toString and use result;

                    // Build final message text
                    set output to do concat and use "Result: ", resultText;

                    // Print it
                    say output;
                }

            } else {
                // --------------------------------------------------
                // SUBTRACTION BRANCH (operation == "-")
                // --------------------------------------------------

                // Read two numbers from the user
                set first to do readNumber and use "First number: ";
                set second to do readNumber and use "Second number: ";

                // "sub" = built-in subtraction
                // result = first - second
                set result to do sub and use first, second;

                // Convert numeric result to string
                set resultText to do toString and use result;

                // Concatenate "Result: " + resultText
                set output to do concat and use "Result: ", resultText;

                // Print the result line
                say output;
            }

        } else {
            // --------------------------------------------------
            // ADDITION BRANCH (operation == "+")
            // --------------------------------------------------
            //
            // We get here because the condition
            // if do neq and use operation, "+"
            // was false, which means operation == "+".

            // Read first and second numbers
            set first to do readNumber and use "First number: ";
            set second to do readNumber and use "Second number: ";

            // "add" = built-in addition
            // result = first + second
            set result to do add and use first, second;

            // Convert number to string for display
            set resultText to do toString and use result;

            // Build output line
            set output to do concat and use "Result: ", resultText;

            // Print the output
            say output;
        }
    }
}
// End of while loop block
// When user types "exit", running becomes 0, loop stops, program ends.