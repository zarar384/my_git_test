package calculator

type Lexer struct {
	input        string
	position     int  // Current symbol
	readPosition int  // Next symbol
	ch           byte // Symbol
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
