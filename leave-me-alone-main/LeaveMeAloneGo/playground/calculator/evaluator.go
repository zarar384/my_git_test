package calculator

func Evaluate(expression Expression) int {
	switch node := expression.(type) {
	case *NumberLiteral:
		return node.Value

	case *PrefixExpression:
		// Evaluate the right expression and apply the prefix operator
		right := Evaluate(node.Right)
		switch node.Operator {
		case "-":
			return -right
		default:
			panic("unknown operator: " + node.Operator)
		}

	case *BinaryExpression:
		// Evaluate the left and right expressions and apply the binary operator
		left := Evaluate(node.Left)
		right := Evaluate(node.Right)

		switch node.Operator {
		case "+":
			return left + right
		case "-":
			return left - right
		case "*":
			return left * right
		case "/":
			if right == 0 {
				panic("division by zero")
			}

			return left / right
		default:
			panic("unknown operator: " + node.Operator)
		}

	default:
		panic("Unknown expression type")
	}

	return 0 // Default return value, should not reach here
}
