package playground

import (
	"LeaveMeAloneGo/playground/calculator"
	"fmt"
)

func CalculatorDemo() {
	token := calculator.Token{
		Type:    calculator.Number,
		Literal: "123",
	}

	fmt.Println(token)
}
