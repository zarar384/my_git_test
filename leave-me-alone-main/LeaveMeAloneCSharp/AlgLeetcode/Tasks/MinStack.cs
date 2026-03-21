//62ms Beats 10.18%
public class MinStack{
    List<int> ints;
    public MinStack()
    {
        ints = new List<int>();
    }

    public void Push(int val) {
        ints.Insert(0, val);    
    }
    
    public void Pop() {
        if(ints.Count >0){
            ints.RemoveAt(0);
        } 
    }
    
    public int Top() {
        return ints.FirstOrDefault();
    }
    
    public int GetMin() {
        return ints.Min();
    }
}

//1ms Beats 99.93%
public class MinStack2{
    private Stack<int> stack;   
    private Stack<int> minStack;
    
    public MinStack2()
    {
        stack = new Stack<int>();
        minStack = new Stack<int>();
    }

    public void Push(int val) {
        stack.Push(val);

        if(stack.Count == 0 ||  val <  minStack.Peek()) 
        {
            minStack.Push(val);
        }
    }
    
    public void Pop() {
        if(stack.Pop() == minStack.Peek()) {
            minStack.Pop();
        }
    }
    
    public int Top() {
        return stack.Peek();
    }
    
    public int GetMin() {
        return minStack.Peek();
    }
}