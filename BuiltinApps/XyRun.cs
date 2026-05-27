using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyBeeDOS.Drivers;

namespace XyBeeDOS.BuiltinApps
{
    class XyRun : App
    {
        string path;
        string[] args;
        int cursor_pointer = 0;
        readonly int step_length = 32/8;
        FileStream file;

        Stack<int> operand = new Stack<int>();
        Stack<int> reverse_stack = new Stack<int>();
        bool reverse_stack_flag = false;
        Stack<int[]> locals = new Stack<int[]>();
        Dictionary<int, int[]> heap = new Dictionary<int, int[]>();
        Stack<int> callstack = new Stack<int>();
        DictionaryWithSameType<int> functions = new DictionaryWithSameType<int>();

        Exception exception = null;
        Stack<int> catch_stack = new Stack<int>();

        public XyRun(string path, string[] args)
        {
            this.path = path;
            this.args = args;
        }

        void oper_mnl()
        {
            var value = val();
            push(value);
        }
        void oper_push()
        {
            var local = val();
            push(load(local));
        }
        void oper_pop()
        {
            var local = val();
            save(local, pop());
        }
        void oper_add()
        {
            int a, b; a = pop(); b = pop();
            push(a + b);
        }
        void oper_sub()
        {
            int a, b; a = pop(); b = pop();
            push(a - b);
        }
        void oper_mul()
        {
            int a, b; a = pop(); b = pop();
            push(a * b);
        }
        void oper_div()
        {
            int a, b; a = pop(); b = pop();
            push(a / b);
        }
        void oper_inc()
        {
            push(pop() + 1);
        }
        void oper_dec()
        {
            push(pop() - 1);
        }
        void oper_and()
        {
            int a, b; a = pop(); b = pop();
            push(a & b);
        }
        void oper_or()
        {
            int a, b; a = pop(); b = pop();
            push(a | b);
        }
        void oper_xor()
        {
            int a, b; a = pop(); b = pop();
            push(a ^ b);
        }
        void oper_not()
        {
            push(~pop());
        }
        void oper_lshift()
        {
            int a, b; a = pop(); b = pop();
            push(a << b);
        }
        void oper_rshift()
        {
            int a, b; a = pop(); b = pop();
            push(a >> b);
        }
        void oper_irshift()
        {
            int a, b; a = pop(); b = pop();
            if (b >= 1) a >>= 1; a &= int.MaxValue;
            push(a >> (b - 1));
        }
        void oper_neg()
        {
            push(-pop());
        }
        void oper_more()
        {
            int a, b; a = pop(); b = pop();
            push(Convert.ToInt32(a > b));
        }
        void oper_less()
        {
            int a, b; a = pop(); b = pop();
            push(Convert.ToInt32(a < b));
        }
        void oper_equals()
        {
            int a, b; a = pop(); b = pop();
            push(Convert.ToInt32(a == b));
        }
        void oper_jmp()
        {
            cursor_pointer = val() * step_length;
        }
        void oper_if()
        {
            int a, b; a = pop(); b = val() * step_length;
            if (a == 0) cursor_pointer = b;
        }
        void oper_malloc()
        {
            int size = pop();
            for (int address = 0; address < heap.Count + 1; address++)
            {
                if (!heap.ContainsKey(address))
                {
                    heap.Add(address, new int[size]);
                    push(address);
                    break;
                }
            }
        }
        void oper_free()
        {
            int address = pop();
            push(Convert.ToInt32(heap.Remove(address)));
        }
        void oper_toheap()
        {
            int address = pop(); int index = pop(); int val = pop();
            heap[address][index] = val;
        }
        void oper_fromheap()
        {
            int address = pop(); int index = pop();
            push(heap[address][index]);
        }
        void oper_func()
        {
            int func_index = val();
            functions.Add(func_index,cursor_pointer);
            step(1);
            step(val());
        }
        void oper_return()
        {
            var prev = callstack.Pop();
            cursor_pointer = prev;
            locals.Pop();
        }
        void oper_try()
        {
            catch_stack.Push(val() * step_length);
        }
        void oper_endtry()
        {
            catch_stack.Pop();
        }
        void oper_syscall()
        {
            XyRunDriver.syscall(val(), ref operand);
        }
        void oper_revflag()
        {
            reverse_stack_flag = val() != 0 ? true : false;
        }
        void oper_reverse()
        {
            while (reverse_stack.Count > 0)
            {
                operand.Push(reverse_stack.Pop());
            }
        }
        void oper_call()
        {
            int func_num = val();
            callstack.Push(cursor_pointer);
            cursor_pointer = functions.Get(func_num);
            locals.Push(new int[val()]);
            step(1);
        }
        void oper_double()
        {
            push(operand.Peek());
        }
        void oper_throw()
        {
            exception = new Exception(Drivers.XyRunDriver.GetStringFromStack(ref operand));
        }
        void oper_mnlarr()
        {
            int count = val();
            for(int i = 0; i < count; i++)
            {
                push(val());
            }
        }

        int execute(int oper_num)
        {
            int status = 0;
            switch(oper_num)
            {
                case 0: oper_mnl(); break;
                case 1: oper_push(); break;
                case 2: oper_pop(); break;
                case 3: oper_add(); break;
                case 4: oper_sub(); break;
                case 5: oper_mul(); break;
                case 6: oper_div(); break;
                case 7: oper_inc(); break;
                case 8: oper_dec(); break;
                case 9: oper_and(); break;
                case 10: oper_or(); break;
                case 11: oper_xor(); break;
                case 12: oper_not(); break;
                case 13: oper_lshift(); break;
                case 14: oper_rshift(); break;
                case 15: oper_irshift(); break;
                case 16: oper_neg(); break;
                case 17: oper_more(); break;
                case 18: oper_less(); break;
                case 19: oper_equals(); break;
                case 20: oper_jmp(); break;
                case 21: oper_if(); break;
                case 22: oper_malloc(); break;
                case 23: oper_free(); break;
                case 24: oper_toheap(); break;
                case 25: oper_fromheap(); break;
                case 26: oper_func(); break;
                case 27: oper_return(); break;
                case 28: oper_try(); break;
                case 29: oper_endtry(); break;
                case 30: oper_syscall(); break;
                case 31: oper_revflag(); break;
                case 32: oper_reverse(); break;
                case 33: oper_call(); break;
                case 34: oper_double(); break;
                case 35: oper_throw(); break;
                case 36: status = pop(); break;
                case 37: oper_mnlarr(); break;
            }
            return status;
        }

        void step(int step)
        {
            cursor_pointer += step*step_length;
        }

        int read()
        {
            file.Seek(cursor_pointer, SeekOrigin.Begin);
            int res = 0;
            for(int i = 0; i < step_length; i++)
            {
                res <<= 8;
                res |= file.ReadByte();
            }
            file.Seek(cursor_pointer, SeekOrigin.Begin);
            return res;
        }

        int val()
        {
            int val = read();
            step(1);
            return val;
        }

        void push(int val)
        {
            if (reverse_stack_flag) reverse_stack.Push(val);
            else operand.Push(val);
        }

        int pop()
        {
            return operand.Pop();
        }

        int load(int index)
        {
            return locals.Peek()[index];
        }

        void save(int index, int val)
        {
            var local = locals.Pop();
            local[index] = val;
            locals.Push(local);
        }

        public int BeforeRun()
        {
            file = File.OpenRead(path);
            if (val() > 0) return 1;

            if (args.Length == 0 || (args.Length == 1 && args[0].Length == 0)) { operand.Push(0); return 0; }
            
            int args_size = 1;

            for(int i = 0; i < args.Length; i++)
            {
                args_size += args[i].Length + 1;
            }

            operand.Push(args_size);

            for(int i = 0; i<args.Length; i++)
            {
                for(int c = 0; c < args[i].Length; c++)
                {
                    operand.Push(args[i][c]);
                }
                operand.Push(0);
            }

            operand.Reverse();

            return 0;
        }

        public int Quit()
        {
            file.Close();
            return 0;
        }

        public int Run()
        {
            int status = 0;

            try
            {
                status = execute(val());
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            if (exception != null && catch_stack.Count == 0) 
            {
                AppManager.setValue("EXCEPTION", cursor_pointer + " : " + exception.Message);
                return -1; 
            }
            else if(exception != null)
            {
                Drivers.XyRunDriver.SetStringToStack(exception.Message, ref operand);
                cursor_pointer = catch_stack.Pop();
                exception = null;
            }
            return status;
        }
    }
}
