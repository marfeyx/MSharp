say "M# Calculator";

running has value 1;

while running {
    set operation to do readString and use "Operation (+ - * / or exit): ";

    if do eq and use operation, "exit" {
        running has value 0;
        say "Goodbye.";
    } else {
        if do neq and use operation, "+" {
            if do neq and use operation, "-" {
                if do neq and use operation, "*" {
                    if do neq and use operation, "/" {
                        say "Unknown operation. Use +, -, *, /, or exit.";
                    } else {
                        set first to do readNumber and use "First number: ";
                        set second to do readNumber and use "Second number: ";

                        if do eq and use second, 0 {
                            say "Cannot divide by zero.";
                        } else {
                            set result to do div and use first, second;
                            set resultText to do toString and use result;
                            set output to do concat and use "Result: ", resultText;
                            say output;
                        }
                    }
                } else {
                    set first to do readNumber and use "First number: ";
                    set second to do readNumber and use "Second number: ";
                    set result to do mul and use first, second;
                    set resultText to do toString and use result;
                    set output to do concat and use "Result: ", resultText;
                    say output;
                }
            } else {
                set first to do readNumber and use "First number: ";
                set second to do readNumber and use "Second number: ";
                set result to do sub and use first, second;
                set resultText to do toString and use result;
                set output to do concat and use "Result: ", resultText;
                say output;
            }
        } else {
            set first to do readNumber and use "First number: ";
            set second to do readNumber and use "Second number: ";
            set result to do add and use first, second;
            set resultText to do toString and use result;
            set output to do concat and use "Result: ", resultText;
            say output;
        }
    }
}
