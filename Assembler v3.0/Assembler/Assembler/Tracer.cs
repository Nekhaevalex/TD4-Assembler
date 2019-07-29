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

            public point(PointType type, int x, int y, int depth, IOpcode meta)
            {
                this.type = type;
                this.x = x;
                this.y = y;
                this.depth = depth;
                this.meta = meta;
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
                    map[i][j] = new point(PointType.empty, i, j, 0, build[i][j]);
                }
            }
            this.map = new Map(map, build.Length);
        }
        class Map
        {
            private point[][] map;
            private int size;
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
                map[ex][ey].type = PointType.end;
                //Pathfinding
                while (!targetReached)
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size - 1; j++)
                        {
                            if (j < size - 1)
                            {
                                if (map[i][j + 1].type == PointType.empty)
                                {
                                    map[i][j + 1].depth = stepDepth;
                                    map[i][j + 1].type = PointType.waypoint;
                                }
                                else if (map[i][j + 1].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (j > 0)
                            {
                                if (map[i][j - 1].type == PointType.empty)
                                {
                                    map[i][j - 1].depth = stepDepth;
                                    map[i][j - 1].type = PointType.waypoint;
                                }
                                else if (map[i][j - 1].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (i < size)
                            {
                                if (map[i + 1][j].type == PointType.empty)
                                {
                                    map[i + 1][j].depth = stepDepth;
                                    map[i + 1][j].type = PointType.waypoint;
                                }
                                else if (map[i + 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            if (i > 0)
                            {
                                if (map[i - 1][j].type == PointType.empty)
                                {
                                    map[i - 1][j].depth = stepDepth;
                                    map[i - 1][j].type = PointType.waypoint;
                                }
                                else if (map[i - 1][j].type == PointType.end)
                                {
                                    targetReached = true;
                                }
                            }
                            stepDepth++;
                        }
                    }
                }
                int x = ex, y = ey;
                //Path tracing
                while (map[x][y].type != PointType.start)
                {
                    if ((map[x + 1][y].type == PointType.waypoint || map[x + 1][y].type == PointType.start) && map[x + 1][y].depth == stepDepth - 1)
                    {
                        x += 1;
                        stepDepth--;
                        if (map[x][y].type != PointType.start)
                        {
                            map[x][y].type = PointType.fixed_point;
                        }
                    }
                    if ((map[x - 1][y].type == PointType.waypoint || map[x - 1][y].type == PointType.start) && map[x - 1][y].depth == stepDepth - 1)
                    {
                        x -= 1;
                        stepDepth--;
                        if (map[x][y].type != PointType.start)
                        {
                            map[x][y].type = PointType.fixed_point;
                        }
                    }
                    if ((map[x][y + 1].type == PointType.waypoint || map[x][y + 1].type == PointType.start) && map[x][y + 1].depth == stepDepth - 1)
                    {
                        y += 1;
                        stepDepth--;
                        if (map[x][y].type != PointType.start)
                        {
                            map[x][y].type = PointType.fixed_point;
                        }
                    }
                    if ((map[x][y - 1].type == PointType.waypoint || map[x][y - 1].type == PointType.start) && map[x][y - 1].depth == stepDepth - 1)
                    {
                        y -= 1;
                        stepDepth--;
                        if (map[x][y].type != PointType.start)
                        {
                            map[x][y].type = PointType.fixed_point;
                        }
                    }
                }
                //Clearing unused spaces
                for (int i = 0; i<size; i++)
                {
                    for(int j = 0; j<size; j++)
                    {
                        if (map[i][j].type == PointType.waypoint)
                        {
                            map[i][j].type = PointType.empty;
                            map[i][j].depth = 0;
                        }
                    }
                }

            }
            public Map(point[][] map, int size)
            {
                this.map = map;
                this.size = size;
            }
        }
    }
}