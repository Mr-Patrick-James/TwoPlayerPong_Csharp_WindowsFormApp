using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO; // 🆕 Needed for file operations
using System.Windows.Forms;

namespace TwoPlayerPong
{
    public partial class Form1 : Form
    {
        PictureBox leftPaddle, rightPaddle, ball, background;
        Label scoreLabel, timeLabel;
        Label lblP1HighScore, lblP2HighScore;
        Timer gameTimer = new Timer();
        Timer matchTimer = new Timer();
        int ballX = 5, ballY = 5;
        int p1Speed = 0, p2Speed = 0;
        int p1Score = 0, p2Score = 0;
        int p1HighScore = 0, p2HighScore = 0;

        int matchSeconds = 300;
        int speedIncreaseInterval = 10;

        List<string> scoreHistory = new List<string>();

        string highScoreFile = "highscores.txt"; // 🆕 File name

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Width = 800;
            this.Height = 500;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Pong Game with Score Saving";
            this.KeyPreview = true;

            background = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile("background.png"),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(background);

            leftPaddle = new PictureBox
            {
                Image = Image.FromFile("block.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(20, 100),
                Location = new Point(30, this.ClientSize.Height / 2 - 50),
                BackColor = Color.Transparent
            };
            background.Controls.Add(leftPaddle);

            rightPaddle = new PictureBox
            {
                Image = Image.FromFile("block1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(20, 100),
                Location = new Point(this.ClientSize.Width - 50, this.ClientSize.Height / 2 - 50),
                BackColor = Color.Transparent
            };
            background.Controls.Add(rightPaddle);

            ball = new PictureBox
            {
                Image = Image.FromFile("ball.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(25, 25),
                Location = new Point(this.ClientSize.Width / 2 - 12, this.ClientSize.Height / 2 - 12),
                BackColor = Color.Transparent
            };
            background.Controls.Add(ball);

            scoreLabel = new Label
            {
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(this.ClientSize.Width / 2 - 60, 10)
            };
            background.Controls.Add(scoreLabel);

            timeLabel = new Label
            {
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Yellow,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            background.Controls.Add(timeLabel);

            lblP1HighScore = new Label
            {
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - 180, 10),
                Text = "P1 High Score: 0"
            };
            background.Controls.Add(lblP1HighScore);

            lblP2HighScore = new Label
            {
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - 180, 30),
                Text = "P2 High Score: 0"
            };
            background.Controls.Add(lblP2HighScore);

            gameTimer.Interval = 20;
            gameTimer.Tick += GameTick;
            gameTimer.Start();

            matchTimer.Interval = 1000;
            matchTimer.Tick += MatchTick;
            matchTimer.Start();

            LoadHighScores(); // 🆕 Load when game starts
            UpdateScore();
            UpdateTime();

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }

        private void GameTick(object sender, EventArgs e)
        {
            ball.Left += ballX;
            ball.Top += ballY;

            leftPaddle.Top += p1Speed;
            rightPaddle.Top += p2Speed;

            leftPaddle.Top = Math.Max(60, Math.Min(400 - leftPaddle.Height, leftPaddle.Top));
            rightPaddle.Top = Math.Max(60, Math.Min(400 - rightPaddle.Height, rightPaddle.Top));

            int topBound = 60;
            int bottomBound = 390;

            if (ball.Top <= topBound || ball.Bottom >= bottomBound)
                ballY = -ballY;

            if (ball.Bounds.IntersectsWith(leftPaddle.Bounds) || ball.Bounds.IntersectsWith(rightPaddle.Bounds))
                ballX = -ballX;

            if (ball.Left <= 0)
            {
                p2Score++;
                SaveScore();
                ResetBall();
            }
            else if (ball.Right >= this.ClientSize.Width)
            {
                p1Score++;
                SaveScore();
                ResetBall();
            }

            UpdateScore();
        }

        private void MatchTick(object sender, EventArgs e)
        {
            matchSeconds--;

            if (matchSeconds % speedIncreaseInterval == 0 && matchSeconds != 300)
            {
                ballX += ballX > 0 ? 1 : -1;
                ballY += ballY > 0 ? 1 : -1;
            }

            UpdateTime();

            if (matchSeconds <= 0)
            {
                gameTimer.Stop();
                matchTimer.Stop();

                string result = p1Score > p2Score ? "PLAYER 1 WINS!" :
                                p2Score > p1Score ? "PLAYER 2 WINS!" : "IT'S A DRAW!";

                MessageBox.Show(result, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string history = string.Join("\n", scoreHistory);
                MessageBox.Show("Score history:\n" + history, "Score History");

                this.Close();
            }
        }

        private void UpdateTime()
        {
            int minutes = matchSeconds / 60;
            int seconds = matchSeconds % 60;
            timeLabel.Text = $"Time Left: {minutes:D2}:{seconds:D2}";
        }

        private void ResetBall()
        {
            ball.Left = this.ClientSize.Width / 2 - ball.Width / 2;
            ball.Top = this.ClientSize.Height / 2 - ball.Height / 2;
            ballX = ballX > 0 ? -5 : 5;
            ballY = new Random().Next(-4, 5);
        }

        private void UpdateScore()
        {
            scoreLabel.Text = $"P1: {p1Score}  |  P2: {p2Score}";
        }

        private void SaveScore()
        {
            string currentScore = $"P1: {p1Score} | P2: {p2Score}";
            scoreHistory.Add(currentScore);

            // Update high scores
            bool updated = false;

            if (p1Score > p1HighScore)
            {
                p1HighScore = p1Score;
                lblP1HighScore.Text = $"P1 High Score: {p1HighScore}";
                updated = true;
            }

            if (p2Score > p2HighScore)
            {
                p2HighScore = p2Score;
                lblP2HighScore.Text = $"P2 High Score: {p2HighScore}";
                updated = true;
            }

            if (updated)
            {
                SaveHighScores(); // 🆕 Save only when there's new high score
            }
        }

        private void LoadHighScores()
        {
            if (File.Exists(highScoreFile))
            {
                string[] lines = File.ReadAllLines(highScoreFile);
                foreach (string line in lines)
                {
                    if (line.StartsWith("P1="))
                        p1HighScore = int.Parse(line.Substring(3));
                    if (line.StartsWith("P2="))
                        p2HighScore = int.Parse(line.Substring(3));
                }
            }

            lblP1HighScore.Text = $"P1 High Score: {p1HighScore}";
            lblP2HighScore.Text = $"P2 High Score: {p2HighScore}";
        }

        private void SaveHighScores()
        {
            string[] lines = {
                $"P1={p1HighScore}",
                $"P2={p2HighScore}"
            };
            File.WriteAllLines(highScoreFile, lines);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) p1Speed = -6;
            if (e.KeyCode == Keys.S) p1Speed = 6;
            if (e.KeyCode == Keys.Up) p2Speed = -6;
            if (e.KeyCode == Keys.Down) p2Speed = 6;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S) p1Speed = 0;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) p2Speed = 0;
        }
    }
}
