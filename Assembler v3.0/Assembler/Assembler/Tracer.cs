using Opcode;

namespace Assembler
{
    class Tracer
    {

        enum PointType : byte
        {
            waypoint, start, end, bridge, switchPage, empty, fixed_point
        }
        struct point
        {
            public PointType type;
            public int x;
            public int y;
            public int depth;
            public IOpcode meta;
            public bool toBeDeleted;

            public point(PointType type, int x, int y, int depth, IOpcode meta, bool toBeDeleted)
            {
                this.type = type;
                this.x = x;
                this.y = y;
                this.depth = depth;
                this.meta = meta;
                this.toBeDeleted = toBeDeleted;
            }
        }

        private Map map;

        public Tracer(IOpcode[][] build)
        {
            point[][] map = new point[build.Length][];
            for (int i = 0; i < build.Length; i++)
            {
                map[i] = new point[build.Length];
            }
            for (int i = 0; i < build.Length; i++)
            {
                for (int j = 0; j < build[i].Length; j++)
                {
                    map[i][j] = new point(PointType.empty, i, j, 0, build[i][j], false);
                }
            }
            this.map = new Map(map, build.Length);
        }

        class Map
        {
            private point[][] map;
            private int size;

            private int debug_x_pos;
            private int debug_y_pos;

            private void ShiftAndFree(int x, int y)
            {
                point cache;
                int i = size;
                int j = size - 1;
                while (i != x && j != y)
                {
                    if (j != 0)
                    {
                        map[i][j] = map[i][j - 1];
                        j--;
                    }
                    else if (j == 0)
                    {
                        map[i][j] = map[i - 1][size - 1];
                    }
                }
            }
            public point Get(int x, int y)
            {
                return map[x][y];
            }
            private void Trace(int bx, int by, int ex, int ey)
            {
                bool targetReached = false;
                int stepDepth = 1;
                map[bx][by].type = PointType.start;
                map[bx][by].depth = 0;
                map[ex][ey].type = PointType.end;
                //Pathfinding
                debug_x_pos = System.Console.CursorLeft;
                debug_y_pos = System.Console.CursorTop;
                while (!targetReached)
                {
                    if (Program.verboseMode)
                    {
                        System.Console.WriteLine();
                        PrintMap();
                    }
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size - 1; j++)
                        {
                            if (j < size - 2)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i][j + 1].depth = stepDepth;
                                    map[i][j + 1].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (j > 0)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j - 1].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i][j - 1].depth = stepDepth;
                                    map[i][j - 1].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j - 1].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (i < size - 1)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i + 1][j].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i + 1][j].depth = stepDepth;
                                    map[i + 1][j].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i + 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (i > 0)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i - 1][j].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    map[i - 1][j].depth = stepDepth;
                                    map[i - 1][j].type = PointType.waypoint;
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i - 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                        }
                    }
                    stepDepth++;
                }
                int x = ex, y = ey;
                stepDepth--;
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
                //Path tracing
                while (map[x][y].type != PointType.start)
                {
                    if (Program.verboseMode)
                    {
                        System.Console.WriteLine();
                        PrintMap();
                    }
                    if (y < size - 1 && y > 0 && x < size - 1 && x > 0)
                    {
                        if ((map[x][y + 1].type == PointType.waypoint || map[x][y + 1].type == PointType.start) && map[x][y + 1].depth == stepDepth - 1)
                        {
                            y += 1;
                            stepDepth--;
                            if (map[x][y].type != PointType.start)
                            {
                                map[x][y].type = PointType.fixed_point;
                            }
                        }
                        else if ((map[x][y - 1].type == PointType.waypoint || map[x][y - 1].type == PointType.start) && map[x][y - 1].depth == stepDepth - 1)
                        {
                            y -= 1;
                            stepDepth--;
                            if (map[x][y].type != PointType.start)
                            {
                                map[x][y].type = PointType.fixed_point;
                            }
                        }
                        else if ((map[x + 1][y].type == PointType.waypoint || map[x + 1][y].type == PointType.start) && map[x + 1][y].depth == stepDepth - 1)
                        {
                            x += 1;
                            stepDepth--;
                            if (map[x][y].type != PointType.start)
                            {
                                map[x][y].type = PointType.fixed_point;
                            }
                        }
                        else if ((map[x - 1][y].type == PointType.waypoint || map[x - 1][y].type == PointType.start) && map[x - 1][y].depth == stepDepth - 1)
                        {
                            x -= 1;
                            stepDepth--;
                            if (map[x][y].type != PointType.start)
                            {
                                map[x][y].type = PointType.fixed_point;
                            }
                        }
                    }
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
                //Clearing unused spaces
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].type == PointType.waypoint)
                        {
                            map[i][j].type = PointType.empty;
                            map[i][j].depth = 0;
                        }
                    }
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
                //Clearing straight lines
                for (int i = 1; i < size - 1; i++)
                {
                    for (int j = 1; j < size - 1; j++)
                    {
                        bool remove = true;
                        if (map[i][j].type == PointType.fixed_point)
                        {
                            if (map[i - 1][j].type == PointType.fixed_point)
                            {
                                if (map[i][j - 1].type == PointType.fixed_point)
                                {
                                    remove = false;
                                }
                                else if (map[i][j + 1].type == PointType.fixed_point)
                                {
                                    remove = false;
                                }
                            }
                            else if (map[i + 1][j].type == PointType.fixed_point)
                            {
                                if (map[i][j - 1].type == PointType.fixed_point)
                                {
                                    remove = false;
                                }
                                else if (map[i][j + 1].type == PointType.fixed_point)
                                {
                                    remove = false;
                                }
                            }
                        }
                        if (map[i][j].type == PointType.fixed_point && remove)
                        {
                            map[i][j].toBeDeleted = true;
                            map[i][j].depth = 0;
                        }
                    }
                }
                //Clearing unused spaces
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].toBeDeleted)
                        {
                            map[i][j].type = PointType.empty;
                            map[i][j].depth = 0;
                        }
                    }
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
            }
            public Map(point[][] map, int size)
            {
                this.map = map;
                this.size = size;
                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < map[i].Length; j++)
                    {
                        if (map[i][j].meta is Jmp || map[i][j].meta is Jnc)
                        {
                            Trace(i, j, GetLinkedPoint(map[i][j]).x, GetLinkedPoint(map[i][j]).y);
                        }
                    }
                }
            }

            private point GetLinkedPoint(point point)
            {
                int targetAddr = point.meta.FastAdd.toInt();
                int targetI = targetAddr / 16;
                int targetJ = targetAddr % 16;
                point a = new point(PointType.end, targetI, targetJ, 0, map[targetI][targetJ].meta, false);
                return a;
            }

            private void PrintMap()
            {
                System.Console.SetCursorPosition(debug_x_pos, debug_y_pos);
                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < map[i].Length; j++)
                    {
                        switch (map[i][j].type)
                        {
                            case PointType.empty:
                                System.Console.Write(" ");
                                break;
                            case PointType.start:
                                System.Console.ForegroundColor = System.ConsoleColor.Green;
                                System.Console.Write("S");
                                System.Console.ResetColor();
                                break;
                            case PointType.end:
                                System.Console.ForegroundColor = System.ConsoleColor.Red;
                                System.Console.Write("E");
                                System.Console.ResetColor();
                                break;
                            case PointType.fixed_point:
                                System.Console.Write("o");
                                break;
                            case PointType.waypoint:
                                System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                                System.Console.Write(map[i][j].depth);
                                System.Console.ResetColor();
                                break;
                            case PointType.switchPage:
                                System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                                System.Console.Write("S");
                                System.Console.ResetColor();
                                break;
                            case PointType.bridge:
                                System.Console.ForegroundColor = System.ConsoleColor.Magenta;
                                System.Console.Write("B");
                                System.Console.ResetColor();
                                break;
                        }
                    }
                    System.Console.WriteLine();
                }
            }
        }
    }
}