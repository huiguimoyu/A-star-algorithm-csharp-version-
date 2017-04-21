using System;
using System .IO;
using System .Text;
using System .Text .RegularExpressions; //使用正则表达式，找到string中数字的个数
using System .Collections .Generic;     //使用List
using System .Linq;                     //List的二级排序用到了

namespace AStar
    {
    //
    class Node
        {
        private Node father;    //父节点
        internal Node Father
            {
            get { return father; }
            set { father = value; }
            }
        //cost
        private int g_cost;
        public int G_cost
            {
            get { return g_cost; }
            set { g_cost = value; }
            }
        private int h_cost;
        public int H_cost
            {
            get { return h_cost; }
            set { h_cost = value; }
            }
        private int f_cost;
        public int F_cost
            {
            get { return f_cost; }
            set { f_cost = value; }
            }
        //对应地图上的行号列号
        private int i;
        /// <summary>
        /// 列号
        /// </summary>
        public int I
            {
            get { return i; }
            set { i = value; }
            }
        private int j;
        /// <summary>
        /// 行号
        /// </summary>
        public int J
            {
            get { return j; }
            set { j = value; }
            }
        //是否为墙壁
       public bool wall;
        //是否被加入到了close中
       private bool inClose;
       private bool inOpen;

       public bool InOpen
           {
           get { return inOpen; }
           set { inOpen = value; }
           }
       public bool InClose
           {
           get { return inClose; }
           set { inClose = value; }
           }
        public Node()
            {
            father = null;
            wall = false;
            g_cost = 0;
            h_cost = 0;
            f_cost = 0;
            i = 1000;
            j = 1000;
            inClose = false;
            inOpen = false;
            }
        /// <summary>
        /// 外部调用这个函数来给node赋值，初始化时候用
        /// </summary>
        /// <param name="m">行号</param>
        /// <param name="n">列号</param>
        /// <param name="wall">map中的数值</param>
        public void inital(int m , int n , char iswall)
            {
            J = m;
            I = n;
            wall = iswall.Equals('0')?false : true;
            inClose = false;
            }
        }
    //
    class FileTool
        {
        byte[] byData = new byte[ 100 ];
        char[] charData = new char[ 1000 ];
        /// <summary>
        /// 读文件函数
        /// </summary>
        /// <param name="pos">txt文件的具体位置</param>
        public string Read (string path )
            {
            try
                {               
                string str = File .ReadAllText ( path ); 
                return str;
                }
            catch ( IOException e )
                {
                Console .WriteLine ( e .ToString ( ) );
                return null;
                }
            }
        }
    class AStar
        {
            //声明开放列表和关闭列表
            List<Node> openList ;
            List<Node> closeList ;
            //斜对角一格的距离
            const int diagonalDist = 14;
            //横纵一格的距离
            const int straightDist = 10;
            /// <summary>
            /// 起点列号
            /// </summary>
            private int startX_;
            /// <summary>
            /// 起点行号
            /// </summary>
            private int startY_;      
            /// <summary>
            /// 终点的列号
            /// </summary>
            private int endX_;
            /// <summary>
            /// 终点的行号
            /// </summary>   
            private int endY_;   
            /// <summary>
            /// 列号
            /// </summary>
            int tempX;
            /// <summary>
            /// 行号
            /// </summary>
            int tempY;        
            /// <summary>
            /// 当前访问的节点
            /// </summary>
            Node curNode;
            public AStar()
                {
                openList = new List<Node> ( );
                closeList = new List<Node> ( );
                tempX = 0;
                tempY = 0;
                //不初始化curNode，用它来获得其他Node的引用，会报错吗？
                }
            /// <summary>
            /// 将起点终点的坐标传递进来
            /// </summary>
            /// <param name="sx">起点列</param>
            /// <param name="sy">起点行</param>
            /// <param name="ex">终点列</param>
            /// <param name="ey">终点行</param>
            public void SetPointCoor(int sx, int sy , int ex, int ey)
                {
                startX_ = sx;
                startY_ = sy;
                endX_ = ex;
                endY_ = ey;
                }
            /// <summary>
            /// 判断该坐标上的Node能否加入到List中，能则加入，不能则不管
            /// </summary>
            /// <param name="posX">列号</param>
            /// <param name="posY">行号</param>
            /// <param name="map">map二维数组</param>
            private void AddNewNodeToList ( Node nextNode, Node preNode )
                {
                //如果既不是墙壁，也不再close集合中，则可以把该node加入到openList中
                if ( nextNode .wall == false && nextNode .InClose == false && nextNode.InOpen == false)
                    {
                    //如果不是墙壁，加入到开放列表中                            
                    openList .Add ( nextNode );
                    nextNode .InOpen = true;
                    //计算H,G,F          
                    //注意这里！！！！我之前算G_COST这里是有问题的，因为我是直接用的下面这句：
                    //nextNode .G_cost = GetCost ( nextNode .I , nextNode .J , startX_, startY_ );
                    //在看了网上的一些博客后，发现别人都用的是下面这句的思想，即新节点的G_COST是需要用上一个节点的GCOST+移动耗费来计算的，而不是直接用新节点和最原始起点算一个COST来等于GCOST，这是有道理的。
                    nextNode .G_cost = preNode.G_cost + GetCost ( nextNode .I , nextNode .J , preNode.I , preNode.J );
                    nextNode .H_cost = GetCost ( nextNode .I , nextNode .J , endX_ , endY_ );
                    nextNode .F_cost = nextNode .G_cost + nextNode .H_cost;
                    //设置父节点
                    nextNode .Father = preNode;
                    }
                else
                    {                   
                     }
                }
            /// <summary>
            /// A*之找到最短路径算法
            /// </summary>
            /// <param name="map">地图</param>
            /// <param name="mapStr">原始txt读到的string，用于最后修改并且打印路径</param>
            /// <param name="rowNum">地图的行数</param>
            /// <param name="columNum">地图的列数</param>
            public void GetShortestPath ( Node[ , ] map , string mapStr , int rowNum , int columNum )
            {
                //0.将起点加入到开放列表中
                openList .Add ( map[ startY_ , startX_ ] );
                //int curPosX,curPosY;//temp use，X：列，Y：行
                #region do--while循环体
                do
                    {                    
                    openList .Sort ( SortByFHcost );                   
                    curNode = openList[ 0 ];
                    //将其从open中移除                    
                    openList .Remove ( curNode );
                    curNode .InOpen = false; ;                                    
                    //加入到close列表中
                    //closeList .Add ( curNode );                   
                    //设置开关
                    curNode .InClose = true;
                    //判断是否为终点
                    if( curNode.I == endX_ && curNode.J == endY_ )
                        {
                        Console .WriteLine ( "find path" );
                        break;
                        }                    
                    #region 把邻居加入到开放列表中                   
                    tempY = curNode .J - 1;
                    if ( tempY >= 0 )
                        {
                        //不越界，检查墙壁，可以则加入
                        AddNewNodeToList ( map[ tempY , curNode .I ] , curNode );
                        }
                    //↖
                    tempX = curNode .I - 1;
                    if ( tempX >= 0 && tempY >= 0 )
                        {
                        AddNewNodeToList ( map[ tempY , tempX ] , curNode );
                        }
                    //↗  
                    //tempY = startY - 1//上面已经算了一次，所以这里可以不运算这一步了
                    tempX = curNode .I + 1;
                    if ( tempX < columNum && tempY >= 0 )
                        {
                        AddNewNodeToList ( map[ tempY , tempX ] , curNode );
                        }
                    //左
                    tempX = curNode .I - 1;
                    if ( tempX >= 0 )
                        {
                        AddNewNodeToList (  map[ curNode .J,tempX ] , curNode );
                        }
                    //右
                    tempX = curNode .I + 1;
                    if ( tempX < columNum )
                        {
                        AddNewNodeToList (  map[ curNode .J, tempX ] , curNode );
                        }
                    //下
                    tempY = curNode .J + 1;
                    if ( tempY < rowNum )
                        {
                        AddNewNodeToList ( map[ tempY,curNode .I  ] , curNode );
                        }
                    //↙
                    tempX = curNode .I - 1;
                    if ( tempX >= 0 && tempY < rowNum )
                        {
                        AddNewNodeToList (  map[ tempY ,tempX ] , curNode );
                        }
                    //↘
                    tempX = curNode .I + 1;
                    if ( tempX < columNum && tempY < rowNum )
                        {
                        AddNewNodeToList (  map[ tempY, tempX ] , curNode );
                        }
                    //周围8个全部检查并加入到开放列表完毕
                    #endregion 加入完毕                    
                    }
                while( true );
                #endregion do-while完毕
                //找到最短路径,打印输出
                
                //输出最短路径坐标
            }//end for method
            /// <summary>
            /// 计算从点1到点2的cost
            /// </summary>
            /// <param name="curX">点1列坐标</param>
            /// <param name="curY">点1行坐标</param>
            /// <param name="targetX">点2列坐标</param>
            /// <param name="targetY">点2行坐标</param>
            /// <returns>返回cost</returns>
            private int GetCost(int curX, int curY, int targetX, int targetY)
                {
                int delta1 = Math .Abs ( curX - targetX );
                int delta2 = Math .Abs ( curY - targetY );
                if(delta1 == delta2)
                    {
                    return delta1 * diagonalDist;
                    }
                else if( delta1 < delta2)
                    {
                    return delta1 * diagonalDist + ( delta2 - delta1 ) * straightDist;
                    }
                else
                    {
                    return delta2 * diagonalDist + ( delta1 - delta2 ) * straightDist;
                    }
                }
            /// <summary>
            /// List二级排序，首先用F排一次，然后根据H排一次
            /// </summary>
            /// <param name="a1">元素1</param>
            /// <param name="a2">元素2</param>
            /// <returns>返回排序结果</returns>
            private int SortByFHcost ( Node a1 , Node a2 )
                {
                if ( a1 .F_cost .CompareTo ( a2 .F_cost ) != 0 )
                    return ( a1 .F_cost .CompareTo ( a2 .F_cost ) );
                else if ( a1 .H_cost .CompareTo ( a2 .H_cost ) != 0 )
                    return ( a1 .H_cost .CompareTo ( a2 .H_cost ) );
                else
                    return 1;
                }
            /// <summary>
            /// 在地图中打印最后的路径
            /// </summary>
            /// <param name="mapStr_">地图字符串</param>
            /// <param name="mapColumnNum">地图的列数</param>
            public void PrintPath(string mapStr_, int mapColumnNum)
                {
                    int offset = 0;
                    do
                        {
                            Console .WriteLine ( "最短路径坐标：{0},{1}" , curNode .J , curNode .I );
                            //得到一个坐标，转换到字符串中的位置，然后修改字符串
                            offset = curNode .J * mapColumnNum + curNode .I+curNode .J * 2; //一次换行会占2字符                 
                            //Console .WriteLine ( "offset = {0}" , offset );
                            mapStr_ = mapStr_ .Remove ( offset  , 1 );
                            mapStr_ = mapStr_ .Insert ( offset , "*" );
                           // Console .WriteLine ( mapStr_ );
                            curNode = curNode .Father;
                        }
                    while ( curNode .I != startX_ || curNode .J != startY_ );
                    Console .WriteLine ( mapStr_);
                }
        }
    class Program
        {       
        public static void Run()
            {
            
            FileTool ft = new FileTool ( );
            //读取到数字，保存到str中
            string str = ft .Read ( @"map40x10.txt" );
            //下面分析map
            //获得数字的个数，行数
            int nodeNum   = Regex .Matches ( str , @"\d" ) .Count;//数字
            int mapRowNum = Regex .Matches ( str , @"\r" ) .Count + 1;//行数
            int mapColumns = nodeNum / mapRowNum;//列数
            //下面开始将map.txt-Node化
            //提取string中的每一个位置上个的数字，然后传递给地图上的每一个node，此时也在进行初始化Node的工作，知道了数字的个数，便知道了需要实例化多少个node对象
            //以下这个代码可以取得str中的每一个字符，取得之后，可以赋值给node
            Console .WriteLine ( "数字个数={0},行数={1}，列数={2}" , nodeNum , mapRowNum , mapColumns );
            Console .WriteLine ( str );
            //声明地图二维数组
            Node[,] Map = new Node[ mapRowNum , mapColumns ];
            //下面根据str初始化地图数组
            int start = 0;
            Node tempNode = null;
            //存放临时行列号
            int tempm,tempn,tempp;
            char tenmpChar;
            for ( start = 0; start < nodeNum; start += 1 )
                {
                //计算行列号
                tempp = start;
                tempm = tempp / mapColumns;    //行号
                tempn = tempp % mapColumns;    //列号
                //Console .WriteLine ( "行号 = {0}，列号 = {1}, start={2}" , tempm , tempn ,start);
                //初始化Node
                tempNode = new Node ( );
                tenmpChar = str .ToCharArray ( start + 2*tempm , 1 )[ 0 ];//+2*行数是因为换行符占2个字符
                //Node初始化
                tempNode .inital ( tempm , tempn , tenmpChar );
                //Node初始化完毕，加入到Map中的i , j 位置上
                Map[ tempm , tempn ] = tempNode;
                //Console .WriteLine ( "pos:{0},{1},wall = {2},inClose = {3}" , tempm , tempn , tempNode .wall ,tempNode.InClose);
                }
            //初始化地图操作完毕          
            //准备工作全部完毕，下面开始运行算法
            AStar star = new AStar ( );
            //设置起点坐标，终点坐标
            star .SetPointCoor ( 0 , 0 , 39 , 9 );
            //A*算法计算最短路径并输出路径到屏幕
            star .GetShortestPath ( Map , str , mapRowNum , mapColumns );
            star .PrintPath ( str , mapColumns );           
            }
        static void Main ( string[] args )
            {
            Run ( );           
            }        
        }
    }
