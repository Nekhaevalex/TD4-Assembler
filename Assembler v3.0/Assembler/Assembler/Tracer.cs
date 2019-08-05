using Opcode;
using System;

namespace Assembler
{
    class Tracer
    {

        enum PointType : byte
        {
            waypoint, start, end, bridge, switchPage, empty, fixed_point, old_fixed_point, boundary_point
        }
        class point
        {
            public PointType type;
            public int row;
            public int column;
            public int depth;
            public IOpcode meta;
            public bool toBeDeleted;
            public point target;
            public point owner;
            public bool isStatic;
            public bool isAnalyzed;
            public bool isUserPlaced;
            public point(PointType type, int x, int y, int depth, IOpcode meta, bool toBeDeleted, bool userPlaced)
            {
                this.type = type;
                this.row = x;
                this.column = y;
                this.depth = depth;
                this.meta = meta;
                this.toBeDeleted = toBeDeleted;
                this.target = null;
                this.owner = null;
                this.isStatic = false;
                this.isAnalyzed = false;
                isUserPlaced = userPlaced;
            }

            public void setTarget(point target)
            {
                this.target = target;
            }
            public void setOwner(point target)
            {
                this.owner = target;
            }

            public void makeStatic()
            {
                isStatic = true;
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
                    map[i][j] = new point(PointType.empty, i, j, 0, build[i][j], false, true);
                }
            }
            this.map = new Map(map, build.Length);
        }

        class Map
        {
            private point[][] map;
            private int size;

            private static int debug_x_pos;
            private static int debug_y_pos;

            private void ShiftAndFree(int row, int column)
            {
                point cache;
                int i = size - 1;
                int j = size - 1;
                int nextColumn = column == size - 2 ? 0 : column + 1;
                int nextRow = nextColumn == 0 ? row + 1 : row;
                while (!((i <= row) && (j <= column)))
                {
                    if (j != 0)
                    {
                        if (!map[i][j - 1].isStatic)
                        {
                            map[i][j] = map[i][j - 1];
                            map[i][j].row = i;
                            map[i][j].column = j;
                            j--;
                        }
                        else
                        {
                            int k = j - 1;
                            while (map[i][k].isStatic && k > column)
                            {
                                k--;
                            }
                            map[i][j] = map[i][k];
                            map[i][j].row = i;
                            map[i][j].column = j;
                            j = k;
                        }
                    }
                    else if (j == 0)
                    {
                        map[i][j] = map[i - 1][size - 2];
                        map[i][j].row = i;
                        map[i][j].column = j;
                        i--;
                        j = 14;
                    }
                }
                map[row][column].meta = new Add("a", "0");
                map[row][column].type = PointType.empty;
            }

            private void MoveHelperForward(int row, int column)
            {
                if (column < size - 1)
                {
                    point cache = map[row][column];
                    int nonStaticPointer = column + 1;
                    while (map[row][nonStaticPointer].isStatic)
                    {
                        nonStaticPointer++;
                    }
                    if (nonStaticPointer < 15)
                    {
                        map[row][column] = map[row][nonStaticPointer];
                        map[row][nonStaticPointer] = cache;
                        map[row][column].row = row;
                        map[row][column].column = column;
                        map[row][nonStaticPointer].row = row;
                        map[row][nonStaticPointer].column = nonStaticPointer;
                    }
                }
            }
            public point Get(int x, int y)
            {
                return map[x][y];
            }
            private void Trace(int bx, int by, int ex, int ey)
            {
                Utilities.Utilities.VerbouseOut("TRACER", "Tracing PC changers...");
                bool targetReached = false;
                int stepDepth = 1;
                map[bx][by].type = PointType.start;
                map[bx][by].depth = 0;
                map[ex][ey].type = PointType.end;
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
                //Preventing jumps off (kind of)
                if (ey > 13)
                {
                    MakeAShiftForSaving(ex);
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
                //Pathfinding (we see numbers field)
                debug_x_pos = Console.CursorLeft;
                debug_y_pos = Console.CursorTop;
                int x, y;
                FindPath(ex, ey, ref targetReached, ref stepDepth, out x, out y);
                point LastTraced = map[x][y];
                //Path tracing (we see path)
                TracePath(bx, by, ref stepDepth, ref x, ref y, ref LastTraced);
                //Clearing unused spaces (we see path only)
                ClearUnusedSpaces();
                //Clearing straight lines (we see netpoints)
                ClearStraightLines();
                //Clearing unused spaces && Linking (we see netpoints only)
                ClearUnusedAndLink();
                //Implementing points
                ImplementPoints();
            }

            private void MakeAShiftForSaving(int row)
            {
                ShiftAndFree(row, 14);
            }

            private void ClearUnusedSpaces()
            {
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
            }

            private void ImplementPoints()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        point currentPoint = map[i][j];
                        if ((currentPoint.type == PointType.old_fixed_point || currentPoint.type == PointType.boundary_point) && currentPoint.isAnalyzed == false)
                        {
                            Utilities.Utilities.VerbouseOut("TRACER", "Investigating point: " + currentPoint.column.ToString() + "," + currentPoint.row.ToString());
                            if (currentPoint.owner == null)
                            {
                                //Its start point
                                //If it's JMP, we can replace it with SWM:
                                if (currentPoint.meta is Jmp && currentPoint.target.column == currentPoint.column)
                                {
                                    currentPoint.isAnalyzed = true;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Its JMP and it's target is in the same column: " + currentPoint.target.row.ToString());
                                    currentPoint.meta = new Swi(currentPoint.target.row);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "JMP was replaced with SWM");
                                }
                                /*
                                 * If it's JNC and it's target is not in the same row we must insert construction:
                                 *      JNC (to new help point) <|JMP (over the help point or so called 'bridge'), (new help point)|> ...
                                 * Also we must move target point to the right as it must be above 
                                 */
                                if (currentPoint.meta is Jnc && currentPoint.target.row != currentPoint.row)
                                {
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Its JNC and it's target is in other column: making additional helping point");
                                    int row = currentPoint.row;
                                    int column = currentPoint.column;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Allocating free space...");
                                    ShiftAndFree(row, column + 1);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Done");
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Making bridge in " + (column + 1).ToString() + ":" + row.ToString());
                                    map[row][column + 1].meta = new Jmp(column + 2);
                                    map[row][column + 1].type = PointType.bridge;
                                    map[row][column + 1].isAnalyzed = true;
                                    map[row][column + 1].isUserPlaced = false;
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Allocating free space...");
                                    ShiftAndFree(row, column + 2);
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Done");
                                    Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "Making new helping point");
                                    map[row][column + 2].target = currentPoint.target;
                                    map[row][column + 2].owner = currentPoint;
                                    MoveHelperForward(currentPoint.target.row, currentPoint.target.column);
                                    currentPoint.target = map[row][column + 2];
                                    map[row][column + 2].type = PointType.old_fixed_point;
                                    map[row][column + 2].isAnalyzed = true;
                                    map[row][column].isAnalyzed = true;

                                }
                                if (Program.verboseMode)
                                {
                                    System.Console.WriteLine();
                                    PrintMap();
                                }
                            }
                            else if (currentPoint.target == null)
                            {
                                //Its end point
                                Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "It's end point: nothing to do with it");
                                currentPoint.isAnalyzed = true;
                            }
                            else
                            {
                                //Its help point
                                Utilities.Utilities.VerbouseOut("TRACER_ANALYZER", "It's help point: free and place static");
                                int row = currentPoint.row;
                                int column = currentPoint.column;
                                ShiftAndFree(row, column + 1);
                                map[row][column + 1].meta = new Jmp(column + 2);
                                map[row][column + 1].type = PointType.bridge;
                                map[row][column + 1].isUserPlaced = false;
                                map[row][column + 1].makeStatic();
                                map[row][column + 1].isAnalyzed = true;
                                ShiftAndFree(row, column + 2);
                                MoveHelperForward(row, column);
                                currentPoint.makeStatic();
                                map[row][column + 2].isAnalyzed = true;
                                if (Program.verboseMode)
                                {
                                    System.Console.WriteLine();
                                    PrintMap();
                                }

                            }
                        }
                    }
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
            }

            private void ClearUnusedAndLink()
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i][j].toBeDeleted && map[i][j].type == PointType.fixed_point)
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

            private void ClearStraightLines()
            {
                for (int i = 0; i < size - 1; i++)
                {
                    for (int j = 0; j < size - 1; j++)
                    {
                        bool remove = true;
                        if (map[i][j].type == PointType.fixed_point || map[i][j].type == PointType.boundary_point)
                        {
                            if (i > 0)
                            {
                                if (map[i - 1][j].type == PointType.fixed_point || map[i - 1][j].type == PointType.start || map[i - 1][j].type == PointType.end)
                                {
                                    PointType type = map[i][j - 1].type;
                                    if (j > 0)
                                    {
                                        if (map[i][j - 1].type == PointType.fixed_point || map[i][j - 1].type == PointType.old_fixed_point)
                                        {
                                            remove = false;
                                        }
                                    }
                                    if ((map[i][j + 1].type == PointType.fixed_point || map[i][j + 1].type == PointType.start) || map[i][j + 1].type == PointType.end)
                                    {
                                        remove = false;
                                    }
                                }
                            }
                            if (map[i + 1][j].type == PointType.fixed_point || map[i + 1][j].type == PointType.start || map[i + 1][j].type == PointType.end)
                            {
                                if (j > 0)
                                {
                                    if (map[i][j - 1].type == PointType.fixed_point || map[i][j - 1].type == PointType.old_fixed_point)
                                    {
                                        remove = false;
                                    }
                                }
                                if ((map[i][j + 1].type == PointType.fixed_point || map[i][j + 1].type == PointType.start) || map[i][j + 1].type == PointType.end)
                                {
                                    remove = false;
                                }
                            }
                        }
                        if (map[i][j].type == PointType.start || map[i][j].type == PointType.end)
                        {
                            map[i][j].type = PointType.boundary_point;
                        }
                        if (map[i][j].type == PointType.fixed_point && remove)
                        {
                            map[i][j].toBeDeleted = true;
                            point guyWhoWillBeKilled = map[i][j];
                            if (guyWhoWillBeKilled.owner != null && guyWhoWillBeKilled.target != null)
                            {
                                (guyWhoWillBeKilled.target).owner = guyWhoWillBeKilled.owner;
                            }
                            if (guyWhoWillBeKilled.owner != null && guyWhoWillBeKilled.target != null)
                            {
                                (guyWhoWillBeKilled.owner).target = guyWhoWillBeKilled.target;
                            }
                        }
                        if (map[i][j].type == PointType.fixed_point && !remove)
                        {
                            map[i][j].type = PointType.old_fixed_point;
                        }
                    }
                }
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
            }

            private void TracePath(int bx, int by, ref int stepDepth, ref int x, ref int y, ref point LastTraced)
            {
                while (map[x][y].type != PointType.start)
                {
                    if (Program.verboseMode)
                    {
                        System.Console.WriteLine();
                        PrintMap();
                    }
                    if (y < size - 1 && y >= 0 && x < size - 1 && x >= 0)
                    {
                        if ((map[x][y + 1].type == PointType.waypoint || map[x][y + 1].type == PointType.start) && map[x][y + 1].depth == stepDepth - 1)
                        {
                            y += 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            LastTraced = map[x][y];
                        }
                        else if ((map[x][y - 1].type == PointType.waypoint || map[x][y - 1].type == PointType.start) && map[x][y - 1].depth == stepDepth - 1)
                        {
                            y -= 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            LastTraced = map[x][y];
                        }
                        else if ((map[x + 1][y].type == PointType.waypoint || map[x + 1][y].type == PointType.start) && map[x + 1][y].depth == stepDepth - 1)
                        {
                            x += 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            LastTraced = map[x][y];
                        }
                        else if ((map[x - 1][y].type == PointType.waypoint || map[x - 1][y].type == PointType.start) && map[x - 1][y].depth == stepDepth - 1)
                        {
                            x -= 1;
                            stepDepth--;
                            FixPoint(x, y, LastTraced);
                            LastTraced = map[x][y];
                        }
                        else
                        {
                            stepDepth++;
                        }
                    }
                }
                map[bx][by].setTarget(LastTraced);
                map[bx][by].setOwner(null);
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
            }

            private void FindPath(int ex, int ey, ref bool targetReached, ref int stepDepth, out int x, out int y)
            {
                while (!targetReached)
                {
                    if (Program.verboseMode)
                    {
                        System.Console.WriteLine();
                        PrintMap();
                    }
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            if (j < size - 1)
                            {
                                if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.empty && map[i][j].depth < stepDepth)
                                {
                                    if (j < size - 2)
                                    {
                                        map[i][j + 1].depth = stepDepth;
                                        map[i][j + 1].type = PointType.waypoint;
                                    }
                                }
                                else if ((map[i][j].type == PointType.waypoint || map[i][j].type == PointType.start) && map[i][j + 1].type == PointType.end)
                                {
                                    targetReached = true;
                                    map[i][j + 1].depth = stepDepth;
                                    break;
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
                                    map[i][j - 1].depth = stepDepth;
                                    break;
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
                                    map[i + 1][j].depth = stepDepth;
                                    break;
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
                                    map[i - 1][j].depth = stepDepth;
                                    break;
                                }
                            }
                        }
                        if (targetReached)
                        {
                            break;
                        }
                    }
                    stepDepth++;
                }
                x = ex;
                y = ey;
                stepDepth = map[x][y].depth;
                if (Program.verboseMode)
                {
                    System.Console.WriteLine();
                    PrintMap();
                }
            }

            private void FixPoint(int x, int y, point LastTraced)
            {
                map[x][y].setTarget(LastTraced);
                LastTraced.setOwner(map[x][y]);
                if (map[x][y].type != PointType.start)
                {
                    map[x][y].type = PointType.fixed_point;
                }
            }

            public Map(point[][] map, int size)
            {
                this.map = map;
                this.size = size;
                for (int i = map.Length - 1; i >= 0; i--)
                {
                    for (int j = map[i].Length - 1; j >= 0; j--)
                    {
                        if ((map[i][j].meta is Jmp || map[i][j].meta is Jnc) && map[i][j].isUserPlaced)
                        {
                            Trace(i, j, GetLinkedPoint(map[i][j]).row, GetLinkedPoint(map[i][j]).column);
                        }
                    }
                }
            }

            private point GetLinkedPoint(point point)
            {
                FastAdd target = point.meta.FastAdd;
                int targetAddr = target.toInt();
                int targetI = targetAddr / (Program.eightBit ? 256 : 16);
                int targetJ = targetAddr - (targetI * ((Program.eightBit ? 256 : 16) - 1));
                point a = new point(PointType.end, targetI, targetJ, 0, map[targetI][targetJ].meta, false, false);
                return a;
            }

            private void PrintMap()
            {
                System.Console.SetCursorPosition(debug_x_pos, debug_y_pos);
                System.Console.Write(" /");
                for (int i = -1; i < map.Length; i++)
                {
                    if (i != -1)
                    {
                        if (i < 10)
                        {
                            System.Console.Write(" " + i.ToString());
                        }
                        else
                        {
                            System.Console.Write(i);
                        }
                    }
                    for (int j = 0; j < map.Length; j++)
                    {
                        if (i == -1)
                        {
                            if (j < 10)
                            {
                                System.Console.Write(" " + j.ToString());
                            }
                            else
                            {
                                System.Console.Write(j);
                            }
                        }
                        else
                        {
                            if (map[i][j].meta is Swi && j == 15 && map[i][j].meta.FastAdd.toInt() == i + 1)
                            {
                                System.Console.BackgroundColor = System.ConsoleColor.DarkMagenta;
                                System.Console.Write(">>");
                                System.Console.ResetColor();
                            }
                            switch (map[i][j].type)
                            {
                                case PointType.empty:
                                    System.Console.Write("  ");
                                    break;
                                case PointType.start:
                                    System.Console.ForegroundColor = System.ConsoleColor.Green;
                                    System.Console.Write(" S");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.end:
                                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                                    System.Console.Write(" E");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.fixed_point:
                                    System.Console.Write(" o");
                                    break;
                                case PointType.waypoint:
                                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                                    if (map[i][j].depth < 10)
                                    {
                                        System.Console.Write(" " + map[i][j].depth);
                                    }
                                    else
                                    {
                                        System.Console.Write(map[i][j].depth);
                                    }
                                    System.Console.ResetColor();
                                    break;
                                case PointType.switchPage:
                                    System.Console.ForegroundColor = System.ConsoleColor.Cyan;
                                    System.Console.Write(" S");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.bridge:
                                    System.Console.ForegroundColor = System.ConsoleColor.Magenta;
                                    System.Console.Write(" B");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.old_fixed_point:
                                    System.Console.ForegroundColor = System.ConsoleColor.DarkYellow;
                                    System.Console.Write(" o");
                                    System.Console.ResetColor();
                                    break;
                                case PointType.boundary_point:
                                    System.Console.ForegroundColor = System.ConsoleColor.DarkGreen;
                                    System.Console.Write(" o");
                                    System.Console.ResetColor();
                                    break;
                            }
                        }
                    }
                    System.Console.WriteLine();
                }
            }
        }
    }
}