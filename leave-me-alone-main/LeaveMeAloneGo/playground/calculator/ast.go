package calculator

// AST (Abstract Syntax Tree) representation of the calculator expressions.
/*
Exemple: 1 + 2 * (3 - 4)
Looks like this:
		  +
		 / \
		1   *
		   / \
		  2   -
		     / \
		    3   4
*/

import (
	"fmt"
	"strconv"
)

type Node interface {
	node()
}

type Expression interface {
	Node
	expression()
}

type NumberLiteral struct {
	Token Token
	Value int
}

// Implement the Node interface for NumberLiteral
func (nl *NumberLiteral) node()       {}
func (nl *NumberLiteral) expression() {}
func (nl *NumberLiteral) String() string {
	// strconv.Itoa converts an integer to a string
	// example: strconv.Itoa(42) returns "42"
	return strconv.Itoa(nl.Value)
}

// BinaryExpression represents an expression with two operands and one operator.
//
// Example: 1 + 2
// Token    - the operator token (+)
// Left     - the left operand (e.g. NumberLiteral)
// Operator - the operator symbol ("+", "-", "*", "/")
// Right    - the right operand (e.g. NumberLiteral)
type BinaryExpression struct {
	Token    Token
	Left     Expression
	Operator string
	Right    Expression
}

// Implement the Node interface for BinaryExpression
func (be *BinaryExpression) node()       {}
func (be *BinaryExpression) expression() {}
func (be *BinaryExpression) String() string {
	// $s - string
	return fmt.Sprintf("(%s %s %s)", be.Left, be.Operator, be.Right)
}

// PrefixExpression represents an expression with a prefix operator and one operand.
//
// Example: -5
// Token    - the operator token (-)
// Operator - the operator symbol ("-")
// Right    - the operand (e.g. NumberLiteral)
type PrefixExpression struct {
	Token    Token
	Operator string
	Right    Expression
}

// Implement the Node interface for PrefixExpression
func (pe *PrefixExpression) node()       {}
func (pe *PrefixExpression) expression() {}
func (pe *PrefixExpression) String() string {
	return fmt.Sprintf("(%s%s)", pe.Operator, pe.Right)
}
