package playground

import (
	"LeaveMeAloneGo/playground/calculator"
	"fmt"
)

func CalculatorDemo() {
	lexer := calculator.NewLexer("+-*/()")

	for {
		token := lexer.NextToken()
		fmt.Println(token)

		if token.Type == calculator.EOF {
			break
		}
	}
}
