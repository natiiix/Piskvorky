using System;
using System.Drawing;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class FormMain : Form
    {
        private const int WIN_LENGTH = 5;

        private const int HORIZONTAL_TILES = 16;
        private const int VERTICAL_TILES = 16;

        private int[,] Field;

        private int FieldWidth;
        private int FieldHeight;

        private double TileWidth;
        private double TileHeight;

        private int OnTurn;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            RestartBoard();
        }

        private void RestartBoard()
        {
            Field = new int[HORIZONTAL_TILES, VERTICAL_TILES];
            OnTurn = 1;
            UpdateBoard();
        }

        private void UpdateBoard()
        {
            // Get rid of the old image
            pictureBoxGameField.Image?.Dispose();

            // Create a bitmap for the new image
            Bitmap bmpField = new Bitmap(pictureBoxGameField.Width, pictureBoxGameField.Height);

            // Get the field size
            FieldWidth = bmpField.Width;
            FieldHeight = bmpField.Height;

            // Calculate tile size
            TileWidth = FieldWidth / (double)HORIZONTAL_TILES;
            TileHeight = FieldHeight / (double)VERTICAL_TILES;

            using (Graphics g = Graphics.FromImage(bmpField))
            {
                DrawGrid(g);
                DrawTiles(g);
            }

            // Update the image
            pictureBoxGameField.Image = bmpField;

            // Look for a winner
            TestWinner();
        }

        private void TestWinner()
        {
            for (int y = 0; y < VERTICAL_TILES; y++)
            {
                for (int x = 0; x < HORIZONTAL_TILES; x++)
                {
                    int team = Field[x, y];

                    if (team < 1 || team > 2)
                    {
                        continue;
                    }

                    if (TestHorizontal(x, y, team) ||
                        TestVertical(x, y, team) ||
                        TestDiagonalDown(x, y, team) ||
                        TestDiagonalUp(x, y, team))
                    {
                        MessageBox.Show("Winner: Player " + team.ToString());
                        RestartBoard();
                    }
                }
            }
        }

        private bool TestHorizontal(int x, int y, int team)
        {
            for (int i = 1; i < WIN_LENGTH; i++)
            {
                if (GetTeam(x + i, y) != team)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestVertical(int x, int y, int team)
        {
            for (int i = 1; i < WIN_LENGTH; i++)
            {
                if (GetTeam(x, y + i) != team)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestDiagonalDown(int x, int y, int team)
        {
            for (int i = 1; i < WIN_LENGTH; i++)
            {
                if (GetTeam(x + i, y + i) != team)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestDiagonalUp(int x, int y, int team)
        {
            for (int i = 1; i < WIN_LENGTH; i++)
            {
                if (GetTeam(x + i, y - i) != team)
                {
                    return false;
                }
            }

            return true;
        }

        private int GetTeam(int x, int y) => x >= 0 && y >= 0 && x < HORIZONTAL_TILES && y < VERTICAL_TILES ? Field[x, y] : -1;

        private void DrawGrid(Graphics g)
        {
            // Draw grid
            Pen penGrid = new Pen(Color.Black);

            // Horizontal lines
            double lineY = TileHeight;
            while (lineY < FieldHeight)
            {
                int yRounded = lineY.Round();
                g.DrawLine(penGrid, new Point(0, yRounded), new Point(FieldWidth, yRounded));
                lineY += TileHeight;
            }

            // Vertical lines
            double lineX = TileWidth;
            while (lineX < FieldWidth)
            {
                int xRounded = lineX.Round();
                g.DrawLine(penGrid, new Point(xRounded, 0), new Point(xRounded, FieldHeight));
                lineX += TileWidth;
            }
        }

        private void DrawTiles(Graphics g)
        {
            for (int y = 0; y < VERTICAL_TILES; y++)
            {
                for (int x = 0; x < HORIZONTAL_TILES; x++)
                {
                    DrawPiece(g, TileToPoint(x, y), Field[x, y]);
                }
            }
        }

        private void DrawPiece(Graphics g, Point pieceBase, int team)
        {
            if (team == 0)
            {
                return;
            }

            Pen penPiece = new Pen(
                team == 1 ? Color.Blue :
                team == 2 ? Color.Red :
                Color.Empty, 3f);

            Rectangle rectPiece = new Rectangle(pieceBase, new Size(TileWidth.Round(), TileHeight.Round()));

            g.DrawLine(penPiece, new Point(rectPiece.Left, rectPiece.Top), new Point(rectPiece.Right, rectPiece.Bottom));
            g.DrawLine(penPiece, new Point(rectPiece.Left, rectPiece.Bottom), new Point(rectPiece.Right, rectPiece.Top));
        }

        private Point TileToPoint(int x, int y) => new Point((x * TileWidth).Round(), (y * TileHeight).Round());
        private int[] PointToTile(Point p) => new int[] { (int)(p.X / TileWidth), (int)(p.Y / TileHeight) };

        private void pictureBoxGameField_MouseClick(object sender, MouseEventArgs e)
        {
            int[] tile = PointToTile(e.Location);

            if (Field[tile[0], tile[1]] == 0)
            {
                Field[tile[0], tile[1]] = OnTurn;
                OnTurn = 3 - OnTurn;
                UpdateBoard();
            }
            else
            {
                MessageBox.Show("You cannot override your opponent's piece!");
            }
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            UpdateBoard();
        }
    }

    public static class ExtensionMethods
    {
        public static int Round(this double value) => (int)Math.Round(value);
    }
}
