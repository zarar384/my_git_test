package calculator

import "testing"

func TestParser(t *testing.T) {
	tests := []struct {
		name     string
		input    string
		expected int
	}{
		{
			name:     "number",
			input:    "5",
			expected: 5,
		},
		{
			name:     "addition",
			input:    "1 + 2",
			expected: 3,
		},
		{
			name:     "subtraction",
			input:    "10 - 4",
			expected: 6,
		},
		{
			name:     "multiplication",
			input:    "2 * 3",
			expected: 6,
		},
		{
			name:     "division",
			input:    "8 / 2",
			expected: 4,
		},
		{
			name:     "precedence",
			input:    "1 + 2 * 3",
			expected: 7,
		},
		{
			name:     "parentheses",
			input:    "(1 + 2) * 3",
			expected: 9,
		},
		{
			name:     "prefix",
			input:    "-5",
			expected: -5,
		},
		{
			name:     "complex",
			input:    "-(1 + 2) * 3",
			expected: -9,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			expression := parse(t, tt.input)

			actual := Evaluate(expression)

			if actual != tt.expected {
				t.Errorf("expected %d, got %d", tt.expected, actual)
			}
		})
	}
}
