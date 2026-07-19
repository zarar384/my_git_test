package calculator

import "strconv"

type (
	prefixParseFn func() Expression           // returns an Expression
	infixParseFn  func(Expression) Expression // takes an Expression, returns an Expression
)

type Precedence int

const (
	Lowest  Precedence = iota
	Sum                // + or -
	Product            // * or /
	Prefix             // -X or !X
)

var precedences = map[TokenType]Precedence{
	Plus:     Sum,
	Minus:    Sum,
	Multiply: Product,
	Divide:   Product,
}

type Parser struct {
	lexer *Lexer

	currentToken Token
	peekToken    Token // one token lookahead

	errors []error
}

func NewParser(lexer *Lexer) *Parser {
	p := &Parser{
		lexer: lexer,
	}

	p.nextToken() // Initialize currentToken and peekToken
	p.nextToken() // Move to the first token

	return p
}

func (p *Parser) Errors() []error {
	return p.errors
}

func (p *Parser) nextToken() {
	p.currentToken = p.peekToken
	p.peekToken = p.lexer.NextToken()
}

func (p *Parser) ParseExpression(precedence Precedence) Expression {
	var leftExp Expression

	switch p.currentToken.Type {
	case Number:
		leftExp = p.parseNumberLiteral()

	case LeftParen:
		leftExp = p.parseGroupedExpression()

	case Minus:
		leftExp = p.parsePrefixExpression()

	default:
		return nil
	}

	for p.peekToken.Type != EOF && precedence < p.peekPrecedence() {
		switch p.peekToken.Type {
		case Plus, Minus, Multiply, Divide:
			p.nextToken() // Move to the operator token
			leftExp = p.parseBinaryExpression(leftExp)
		default:
			return leftExp
		}
	}

	return leftExp
}

// precedence of the current token (currentToken)
func (p *Parser) currentPrecedence() Precedence {
	if precedence, ok := precedences[p.currentToken.Type]; ok {
		return precedence
	}

	return Lowest
}

// precedence of the next token (peekToken)
func (p *Parser) peekPrecedence() Precedence {
	if precedence, ok := precedences[p.peekToken.Type]; ok {
		return precedence
	}

	return Lowest
}

// parse a number literal, e.g., 42
func (p *Parser) parseNumberLiteral() *NumberLiteral {
	value, err := strconv.Atoi(p.currentToken.Literal)
	if err != nil {
		return nil // Error handling: invalid number literal
	}

	return &NumberLiteral{
		Token: p.currentToken,
		Value: value,
	}
}

// parse binary expression, e.g., 1 + 2
func (p *Parser) parseBinaryExpression(left Expression) Expression {
	expression := &BinaryExpression{
		Token:    p.currentToken,
		Left:     left,
		Operator: p.currentToken.Literal,
	}

	precedence := p.currentPrecedence()

	p.nextToken() // Move to the right operand

	expression.Right = p.ParseExpression(precedence)

	return expression
}

// parse grouped expression, e.g., (1 + 2)
func (p *Parser) parseGroupedExpression() Expression {
	p.nextToken() // Move to the expression inside the parentheses

	expression := p.ParseExpression(Lowest)

	if p.peekToken.Type != RightParen {
		return nil // Error handling: expected a closing parenthesis
	}

	p.nextToken() // Move to the closing parenthesis

	return expression
}

// parse prefix expression, e.g., -5
func (p *Parser) parsePrefixExpression() Expression {
	expression := &PrefixExpression{
		Token:    p.currentToken,
		Operator: p.currentToken.Literal,
	}

	p.nextToken() // Move to the right operand

	expression.Right = p.ParseExpression(Prefix)

	return expression
}
