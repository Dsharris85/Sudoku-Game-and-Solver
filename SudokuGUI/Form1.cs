using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace SudokuGUI
{
    public partial class Form1 : Form
    {
        private List<List<Label>> _tiles = new List<List<Label>>();
        private Solver _solver = new Solver();
        private Timer _timer = new Timer();

        private int[,] _board;
        private int[,] _startingBoard;
        private int[,] _modelBoard;
        private int[] _selected = { -1, -1 };

        private string _saveFile = @"\sudokuSaves\save.txt";
        private string _wrongAnswers = "";
        private int _timeInSec = 0;
        private string _initials;
        public string Initials { get => _initials; set => _initials = value; }
        public string SaveFile { get => _saveFile;}

        public Form1()
        {
            InitializeComponent();
            _board = _solver.RandomBoard(this.trackBar1.Value);
            _startingBoard = CopyBoard(_board);
            _modelBoard = CopyBoard(_board);

            SetToolTip();

            InitTiles(_modelBoard);
            label1.Text = "Time = 0:0";
            label2.Text = _wrongAnswers;

            TieEventsToLabels();

            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(TimeTick);
            _timer.Start();

            CheckSaves();
        }

        private void CheckSaves()
        {
            if (!Directory.Exists(@"\sudokuSaves"))
            {
                Directory.CreateDirectory(@"\sudokuSaves");

                if (!File.Exists(SaveFile))
                {
                    File.Create(SaveFile);
                }
            }
        }
        public void LoadGame(string game)
        {
            _timer.Start();
            label2.ForeColor = Color.Red;
            label2.Text = " ";
            int[,] arr = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _tiles[i][j].BackColor = SystemColors.ButtonFace;
                    arr[i,j] = (int)Char.GetNumericValue(game[(i * 9) + j]);
                }
            }
            _solver.PrintBoard(arr);
            label1.Text = "Time = 0:0";

            _board = CopyBoard(arr);
            _startingBoard = CopyBoard(_board);
            _modelBoard = CopyBoard(_board);

            UpdateTiles(_board);

            _timeInSec = 0;
        }
        private void SetToolTip()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
        }
        private int[,] CopyBoard(int[,] bo)
        {
            int[,] arr = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arr[i, j] = bo[i, j];
                }
            }
            return arr;
        }
        private void TieEventsToLabels()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _tiles[i][j].MouseEnter += TileMouseEnter;
                    _tiles[i][j].MouseLeave += TileMouseLeave;
                    _tiles[i][j].MouseClick += TileMouseClick;
                }
            }
        }
        private string TimeFormatted()
        {
            int min = _timeInSec / 60;
            int sec = _timeInSec % 60;
            return $"Time = {min}:{sec}";
        }
        private void TimeTick(object sender, EventArgs eArgs)
        {
            _timeInSec++;
            label1.Text = TimeFormatted();
        }
        private void InitTiles(int[,] bo)
        {
            List<Label> l = new List<Label>();

            foreach (Control x in this.Controls)
            {
                if (x.Name.Contains("tile"))
                {
                    l.Insert(0, (Label)x);
                }
            }

            List<Label> SortedList = l.OrderBy(o => Int32.Parse((o.Name).Substring(4))).ToList();

            for (int i = 0; i < 9; i++)
            {
                _tiles.Add(new List<Label>());

                for (int j = 0; j < 9; j++)
                {
                    _tiles[i].Add(new Label());
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _tiles[i][j] = SortedList[(i * 9) + j];
                    if (bo[i, j] == 0)
                    {
                        _tiles[i][j].Text = " ";
                    }
                    else
                    {
                        _tiles[i][j].Text = bo[i, j].ToString();
                    }

                }
            }
        }
        private void ResetBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _tiles[i][j].BackColor = SystemColors.ButtonFace;
                }
            }

            label1.Text = "Time = 0:0";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _board[i, j] = _startingBoard[i, j];
                }
            }
            _startingBoard = CopyBoard(_board);
            _modelBoard = CopyBoard(_board);
            UpdateTiles(_board);

            _timeInSec = 0;
        }
        private void WinGame(bool helped)
        {
            label2.ForeColor = Color.Green;
            label2.Text = "You Won!";
            _timer.Stop();

            if (!helped)
            {
                bool high = CheckIfHighScore();
                if (high)
                {
                    HighScorePrompts prompt = new HighScorePrompts(this);
                    this.Hide();
                    prompt.Show();
                }               
            }
        }
        private string CopyBoardToWrite()
        {
            string bo = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    bo += _startingBoard[i, j].ToString();
                }
            }
            return bo;
        }
        public void AddScoreToFile()
        {
            List<string> lines = File.ReadAllLines(SaveFile).ToList();
            string bo = CopyBoardToWrite();

            // not enough in list yet 
            if (lines.Count < 10)
            {
                lines.Add($"{_initials} {this.trackBar1.Value} {_timeInSec} {bo}");
                lines = lines.OrderBy(x => x[1]).ToList();
                File.WriteAllLines(SaveFile, lines);
            }
            //need to find which score to replace
            else
            {
                List<string> arr = new List<string>();
                bool added = false;
                foreach (string line in lines)
                {
                    String[] elements = line.Split(' ');
                    int difficulty = Int32.Parse(elements[1]);
                    int time = Int32.Parse(elements[2]);

                    // if same or less difficulty and shorter time...                    
                    if (difficulty <= this.trackBar1.Value & _timeInSec < time & added == false)
                    {
                        arr.Add($"{_initials} {this.trackBar1.Value} {_timeInSec} {bo}");
                        added = true;
                    }
                    else
                    {
                        arr.Add(line);
                    }                    
                }
                File.WriteAllLines(SaveFile, arr);
            }
        }
        private bool CheckIfHighScore()
        {
            List<string> lines = File.ReadAllLines(SaveFile).ToList();

            // not enough in list yet 
            if (lines.Count < 10)
            {
                return true;
            }
            // leaderboard filled, check if replace need
            foreach (string line in lines)
            {
                String[] elements = line.Split(' ');
                int difficulty = Int32.Parse(elements[1]);

                if (difficulty <= this.trackBar1.Value)
                {
                    int time = Int32.Parse(elements[2]);
                    if (_timeInSec < time)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        // SOLVE
        private void button3_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    _tiles[x][y].BackColor = SystemColors.ButtonFace;
                }
            }

            _solver.SolveBoard(ref _board);                      

            UpdateTiles(_board);

            WinGame(true);
        }       
        // RESET
        private void button2_Click(object sender, EventArgs e)
        {
            ResetBoard();
            label2.Text = " ";
            label2.ForeColor = Color.Red;
            _timer.Start();
        }
        // NEW GAME
        private void button1_Click(object sender, EventArgs e)
        {
            _timer.Start();
            label2.ForeColor = Color.Red;
            label2.Text = " ";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _tiles[i][j].BackColor = SystemColors.ButtonFace;
                }
            }
            label1.Text = "Time = 0:0";

            _board = _solver.RandomBoard(this.trackBar1.Value);
            _startingBoard = CopyBoard(_board);
            _modelBoard = CopyBoard(_board);

            UpdateTiles(_board);

            _timeInSec = 0;            
        }
        private void UpdateTiles(int[,] bo)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (bo[i, j] == 0)
                    {
                        _tiles[i][j].Text = " ";
                    }
                    else
                    {                        
                        _tiles[i][j].Text = bo[i, j].ToString();
                    }
                }
            }
        }
        private void TileMouseEnter(object sender, EventArgs e)
        {
            var lab = sender as Label;
            if (lab.Text == " " & lab.BackColor != SystemColors.Highlight)
            {
                lab.BackColor = SystemColors.InactiveCaption; //darkercolor
            }
        }
        private void TileMouseLeave(object sender, EventArgs e)
        {
            var lab = sender as Label;
            if (lab.Text == " ")
            {
                if (lab.BackColor != SystemColors.Highlight)
                {
                    lab.BackColor = SystemColors.ButtonFace; //lightercolor

                }
            }
        }
        private void TileMouseClick(object sender, EventArgs e)
        {
            var lab = sender as Label; 
            int loc = lab.GetHashCode();

            // look @ each row for label clicked (loc)
            for (int i = 0; i < 9; i++)
            {
                // Check row for label clicked
                for (int j = 0; j < 9; j++)
                {
                    // if on tile found, change color to highlight
                    if (_tiles[i][j].GetHashCode() == loc)
                    {
                        Debug.WriteLine(i.ToString(),j.ToString());
                        _selected[0] = i;
                        _selected[1] = j;

                        if (_board[i,j] == 0)                      
                            lab.BackColor = SystemColors.Highlight; //blue selected                        
                        else
                        {
                            _selected[0] = -1;
                            _selected[1] = -1;
                        }
                    }
                    // for every other tile, unhighlight
                    else
                    {
                        if (_tiles[i][j].BackColor == SystemColors.Highlight)
                            _tiles[i][j].BackColor = SystemColors.ButtonFace;//white
                    }
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // If valid tile to select
            if (_selected[0] != -1)
            {
                int x = _selected[0];
                int y = _selected[1];
                // D1-D9
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        _tiles[x][y].Text = 1.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D2:
                        _tiles[x][y].Text = 2.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D3:
                        _tiles[x][y].Text = 3.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D4:
                        _tiles[x][y].Text = 4.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D5:
                        _tiles[x][y].Text = 5.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D6:
                        _tiles[x][y].Text = 6.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.D7:
                        _tiles[x][y].Text = 7.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);

                        break;
                    case Keys.D8:
                        _tiles[x][y].Text = 8.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x, y] = Int32.Parse(_tiles[x][y].Text);

                        break;
                    case Keys.D9:
                        _tiles[x][y].Text = 9.ToString();
                        _tiles[x][y].BackColor = Color.LightSeaGreen;
                        _modelBoard[x,y] = Int32.Parse(_tiles[x][y].Text);
                        break;
                    case Keys.S:
                        if(TrySet(_tiles[x][y], x, y))
                        {
                            _tiles[x][y].BackColor = SystemColors.ButtonFace;
                        }
                        else
                        {
                            _tiles[x][y].Text = " ";
                            _tiles[x][y].BackColor = Color.Red;

                            if (label2.Text.Length < 6)
                            {
                                label2.Text += "X";
                            }
                            else
                            {
                                label2.Text = "";
                                ResetBoard();
                            }
                            
                        }
                        int count = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                if (_board[i,j] == 0)
                                {
                                    count++;
                                }
                            }
                        }
                        if(count == 0)
                        {
                            WinGame(false);
                        }
                        break;
                    default:                        
                        break;
                }
            }            
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(trackBar1, $"Difficulty = {trackBar1.Value.ToString()}");
        }
        private bool TrySet(Label lbl, int x, int y)
        {
            if (x != -1 & lbl.Text != " ")
            {
                // copy of board plus new num to test
                int[,] arr = CopyBoard(_board);

                _solver.PrintBoard(_board);

                if (_solver.ValidBoard(_board, Int32.Parse(lbl.Text), x, y))
                {
                    arr[x, y] = Int32.Parse(lbl.Text);
                    Debug.WriteLine("Valid board");
                    if (_solver.SolveBoard(ref arr)) 
                    {
                        _board[x, y] = Int32.Parse(lbl.Text);
                        return true;
                    }
                    else
                    {                        
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        //High Scores
        private void button4_Click(object sender, EventArgs e)
        {
            Scores _highScores = new Scores(this);
            this.Hide();
            _highScores.Show();
        }
    }

    
}
