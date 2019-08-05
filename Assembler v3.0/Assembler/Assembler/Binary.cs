using Opcode;

namespace Assembler
{
    class Binary
    {
        private IOpcode[][] matrixStack;
        private IOpcode[] arrayStack;

        public Binary(IOpcode[][] stack)
        {
            matrixStack = stack;
            arrayStack = new IOpcode[stack.Length * stack.Length];
            for (int i = 0; i < matrixStack.Length; i++)
            {
                for (int j = 0; j < matrixStack[i].Length; j++)
                {
                    arrayStack[i * stack.Length + j] = matrixStack[i][j];
                }
            }
        }
        public IOpcode[][] GetMatrixStack()
        {
            return matrixStack;
        }
        public IOpcode[] GetArrayStack()
        {
            return arrayStack;
        }

        public IOpcode this[int key]
        {
            get
            {
                return arrayStack[key];
            }
        }

        public int Count
        {
            get
            {
                return arrayStack.Length;
            }
        }

        public byte[] MakeBuild()
        {
            byte[] build;
            if (Program.eightBit)
            {
                build = new byte[arrayStack.Length * 3];
                for (int i = 0; i < arrayStack.Length; i++)
                {
                    int line;
                    if (arrayStack[i] != null)
                    {
                        line = arrayStack[i].toMachineCode().MachineCode8bit();
                    }
                    else
                    {
                        line = new Add("a", "0").toMachineCode().MachineCode8bit();
                    }
                    build[i * 3] = (byte)((line & 0b111100000000) >> 8);
                    build[i * 3 + 1] = (byte)((line & 0b11110000) >> 4);
                    build[i * 3 + 2] = (byte)((line & 0b1111));
                }
            }
            else
            {
                build = new byte[arrayStack.Length];
                for (int i = 0; i < arrayStack.Length; i++)
                {
                    if (arrayStack[i] != null)
                    {
                        build[i] = arrayStack[i].toMachineCode().MachineCode4bit();
                    }
                    else
                    {
                        build[i] = new Add("a","0").toMachineCode().MachineCode4bit();
                    }
                }
            }
            return build;
        }
    }
}
