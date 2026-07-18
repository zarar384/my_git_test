package calculator

type Lexer struct {
	input        string
	position     int  // Current symbol
	readPosition int  // Next symbol
	ch           byte // Current character.
}

func NewLexer(input string) *Lexer {
	l := &Lexer{input: input}
	l.readChar() // Initialize the current character
	return l
}

func (l *Lexer) readChar() {
	if l.readPosition >= len(l.input) { // End of input
		l.ch = 0 // ASCII code for NUL, signifies end of input
	} else { // Read the next character
		l.ch = l.input[l.readPosition]
	}

	// Move to the next position
	l.position = l.readPosition
	l.readPosition++
}

// reads a number from the input and returns it as a string
func (l *Lexer) readNumber() string {
	start := l.position

	for isDigit(l.ch) {
		l.readChar()
	}

	// Return the substring representing the number
	// example:
	// if input is "123+456",
	// current position is at '1',
	// it will read until it reaches '+',
	// return "123"
	return l.input[start:l.position]
}

// NextToken returns the next token from the input.
func (l *Lexer) NextToken() Token {
	l.skipWhitespace()

	var token Token

	switch l.ch {
	case '+':
		token = newToken(Plus, l.ch)
	case '-':
		token = newToken(Minus, l.ch)
	case '*':
		token = newToken(Multiply, l.ch)
	case '/':
		token = newToken(Divide, l.ch)
	case '(':
		token = newToken(LeftParen, l.ch)
	case ')':
		token = newToken(RightParen, l.ch)
	case 0:
		token = Token{
			Type: EOF,
		}
	default:
		if isDigit(l.ch) {
			return Token{
				Type:    Number,
				Literal: l.readNumber(),
			}
		}

		token = Token{
			Type:    Illegal,
			Literal: string(l.ch),
		}
	}

	l.readChar()

	return token
}

// skip over whitespace characters (spaces, tabs, newlines) in the input.
func (l *Lexer) skipWhitespace() {
	for l.ch == ' ' || l.ch == '\t' || l.ch == '\n' || l.ch == '\r' {
		l.readChar()
	}
}
