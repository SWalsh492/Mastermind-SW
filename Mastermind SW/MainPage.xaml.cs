using Newtonsoft.Json;
using System.Reflection;

namespace Mastermind_SW;


public class GameState // GameState class for file saving/writing
{
    public int[] playerRows;
    public int currentRow;

    public GameState()
    {
        playerRows = new int[11];
        BoxView boxView1 = new BoxView();
        BoxView boxView2 = new BoxView();
        BoxView boxView3 = new BoxView();
        BoxView boxView4 = new BoxView();
        for (int i = 0; i < 4; i++)
        {
            BoxView[] gameRow = { boxView1, boxView2, boxView3, boxView4 };
        }
    }
}
public partial class MainPage : ContentPage
{
    // FIELDS
    // File & Gamestate Fields
    private string filename = "MastermindFile.txt";
    private GameState _gameState;
    int[] colourSaves = { 0, 0, 0, 0 };

    // Global Random declaration
    private Random _random;

    // Vars for guessing functionality
    private Button currentGuessPeg;
    int currentRow = 0;
    int blackPegs = 0;
    int whitePegs = 0;
    int noPegs = 0;
    // Colour array
    Color[] myColours = { Colors.Red, Colors.Green, Colors.Yellow, Colors.Blue , Colors.Orange, Colors.Purple};

    // Arrays for the code and the users guess, each int is for a position of myColours
    int[] code = {0, 0, 0, 0};
    int[] playerGuess = { 0, 0, 0, 0 };
    
	public MainPage()
	{
		InitializeComponent();
        OnStartUp();
    }

    private void OnStartUp()
    {
        // Displays welcome message, removes other elements
        if (LblWelcome.IsVisible == true)
        {
            Board.IsVisible = false;
            PegBoard.IsVisible = false;
        }

        // Initiates random and assigns random colours for start of game display
        _random = new Random();
        BtnGuess1.BackgroundColor = myColours[_random.Next(0, 6)];
        BtnGuess2.BackgroundColor = myColours[_random.Next(0, 6)];
        BtnGuess3.BackgroundColor = myColours[_random.Next(0, 6)];
        BtnGuess4.BackgroundColor = myColours[_random.Next(0, 6)];
        GenerateCode();
        DisplayCode();
    }

    // Functionality for the colour selector
    private void Button_Clicked(object sender, EventArgs e)
    {
        if (currentGuessPeg != null) currentGuessPeg.BorderColor = Colors.Transparent;

        currentGuessPeg = (Button)sender;
        currentGuessPeg.BorderColor = Colors.Magenta;
        GridColourPicker.IsVisible = true;
    }

    private void ColorGuessBoxView_Tapped(object sender, EventArgs e)
    {
        BoxView b = (BoxView)sender;
        currentGuessPeg.BackgroundColor = b.Color;
        currentGuessPeg.BorderColor = Colors.Transparent;
        GridColourPicker.IsVisible = false;
    }

    private void BtnDismissWelcome_Clicked(object sender, EventArgs e)
    {
        // Dismisses the welcome and brings up rest of game
        BtnDismissWelcome.IsVisible = false;
        LblWelcome.IsVisible = false;
        Board.IsVisible = true;
        PegBoard.IsVisible = true;
    }

    public void GenerateCode()
    {
        for(int i = 0; i < 4; i++)
        {
            code[i] = _random.Next(0,6); // generate a random selection of ints into code[]
        }
    }

    public void DisplayCode()
    { // Uses code[] to display colours into boxviews, this is displayed after games are played
        CodeSlot1.Color = myColours[code[0]];
        CodeSlot2.Color = myColours[code[1]];
        CodeSlot3.Color = myColours[code[2]];
        CodeSlot4.Color = myColours[code[3]];
    }

    // Takes in users guess and translates the colours selected to ints relevant to myColours
    private void Guess_Clicked(object sender, EventArgs e)
    {
        Button[] btnList = { BtnGuess1, BtnGuess2, BtnGuess3, BtnGuess4 }; // for stepping through the guess buttons and filling old pegs
        BoxView[] pegList = { Peg1, Peg2, Peg3, Peg4 }; // Generates pegs for filling old pegs
        for (int i = 0; i < 4; i++)
        {
            if (btnList[i].BackgroundColor == Colors.Red) playerGuess[i] = 0;
            if (btnList[i].BackgroundColor == Colors.Green) playerGuess[i] = 1;
            if (btnList[i].BackgroundColor == Colors.Yellow) playerGuess[i] = 2;
            if (btnList[i].BackgroundColor == Colors.Blue) playerGuess[i] = 3;
            if (btnList[i].BackgroundColor == Colors.Orange) playerGuess[i] = 4;
            if (btnList[i].BackgroundColor == Colors.Purple) playerGuess[i] = 5;
            //GuessDebug.Text += " " + playerGuess[i];

        }
        Check(playerGuess);

        FillPreviousGuess(btnList); 
        FillPreviousPegs(pegList); 

        NextTurn(btnList, pegList);
    }

    // Checks each position of playerGuess and compares against the code, increasing blackPeg or whitePeg variables
    private void Check(int[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if (array[i] == code[i])
                blackPegs++;
            else
            {
                for(int j= 0; j<array.Length; j++)
                {
                    if (array[i] == code[j]) 
                        whitePegs++;
                }
            }
        }     
        CalculatePegs();
    }

    // Calculates what colour to make the pegs based on amount in each variable
    private void CalculatePegs()
    {
        BoxView[] PegList = { Peg1, Peg2, Peg3, Peg4 };
        noPegs = 4 - (blackPegs + whitePegs); // NoPegs variable for visually filling empty space
        for (int i = 0; i < 4; i++)
        {
            if (blackPegs > 0)
            {
                PegList[i].Color = Colors.Black; // Places blacks first if there are any
                blackPegs--;
            }
            else if (whitePegs > 0)
            {
                PegList[i].Color = Colors.White; // Then places whites after
                whitePegs--;
            }
            else if (noPegs > 0)
            {
                PegList[i].Color = Colors.DarkGray; // Fills empty space in pegs with Darkgray
            }
        }
    }

    // Sets up board for users next turn, including
    private void NextTurn(Button[] btnList, BoxView[] pegList)
    {
        // Increases currentRow var and triggers win/lose check
        currentRow++;
        CheckforWin();

        for (int i = 0; i < 4; i++)
        {
            btnList[i].SetValue(Grid.RowProperty, currentRow);
            pegList[i].SetValue(Grid.RowProperty, currentRow);
            pegList[i].Color = Colors.DarkGray; //Sets pegs to darkgray for currentrows
        }
    }

    // If users guess is correct, displays win Message. Otherwise game continues or on final row game ends and lose Message displayed
    private void CheckforWin()
    {
        BoxView[] codeSlots = { CodeSlot1, CodeSlot2, CodeSlot3, CodeSlot4 };
        if (playerGuess[0] == code[0] && playerGuess[1] == code[1] && playerGuess[2] == code[2] && playerGuess[3] == code[3])
        {
            Board.IsVisible = false;
            PegBoard.IsVisible = false;
            WinLbl.IsVisible = true;
            PlayAgain.IsVisible = true;
            for (int i = 0; i < 4; i++)
            {
                codeSlots[i].IsVisible = true; // Displays code after winning
            }
        }
        else if (currentRow == 10)
        {
            Board.IsVisible = false;
            PegBoard.IsVisible = false;
            LoseLbl.IsVisible = true;
            PlayAgain.IsVisible = true;
            for (int i = 0; i < 4; i++)
            {
                codeSlots[i].IsVisible = true; // Displays code after losing
            }
        }
    }

    // Fills in the previous guess for looking back on guesses and comparing
    private void FillPreviousGuess(Button[] btnList)
    {
        BoxView boxView1 = new BoxView();
        BoxView boxView2 = new BoxView();
        BoxView boxView3 = new BoxView();
        BoxView boxView4 = new BoxView();

        BoxView[] previousRow = { boxView1, boxView2, boxView3, boxView4 };

        for (int i = 0; i < 4; i++)
        {
            Board.Children.Add(previousRow[i]);
            previousRow[i].SetValue(Grid.ColumnProperty, i);
            previousRow[i].SetValue(Grid.RowProperty, currentRow);
            previousRow[i].SetValue(BoxView.CornerRadiusProperty, 30);
            previousRow[i].SetValue(BoxView.HeightRequestProperty, 20);
            previousRow[i].SetValue(BoxView.WidthRequestProperty, 20);
            // Assigns colours appropriately based on last guess made
            if (btnList[i].BackgroundColor == Colors.Red) {
                previousRow[i].Color = Colors.Red;
                colourSaves[i] = 0;
            }
            if (btnList[i].BackgroundColor == Colors.Green) {
                previousRow[i].Color = Colors.Green;
                colourSaves[i] = 1;
            }
            if (btnList[i].BackgroundColor == Colors.Yellow) {
                previousRow[i].Color = Colors.Yellow;
                colourSaves[i] = 2;
            }
            if (btnList[i].BackgroundColor == Colors.Blue) {
                previousRow[i].Color = Colors.Blue;
                colourSaves[i] = 3;

            }
            if (btnList[i].BackgroundColor == Colors.Orange) {
                previousRow[i].Color = Colors.Orange;
                colourSaves[i] = 4;

            }
            if (btnList[i].BackgroundColor == Colors.Purple) {
                previousRow[i].Color = Colors.Purple;
                colourSaves[i] = 5;
            }

        }
    }

    // Fills in the previous pegs for looking back on guesses and comparing
    private void FillPreviousPegs(BoxView[] pegList)
    {
        BoxView oldPeg1 = new BoxView();
        BoxView oldPeg2 = new BoxView();
        BoxView oldPeg3 = new BoxView();
        BoxView oldPeg4 = new BoxView();

        for (int i = 0; i < 4; i++)
        {
            BoxView[] previousPegs = { oldPeg1, oldPeg2, oldPeg3, oldPeg4 };
            PegBoard.Children.Add(previousPegs[i]);
            previousPegs[i].SetValue(Grid.ColumnProperty, i);
            previousPegs[i].SetValue(Grid.RowProperty, currentRow);
            previousPegs[i].SetValue(BoxView.CornerRadiusProperty, 30);
            previousPegs[i].SetValue(BoxView.HeightRequestProperty, 10);
            previousPegs[i].SetValue(BoxView.WidthRequestProperty, 10);
            // Assigns pegs appropriately based on last guess made
            if (pegList[i].Color == Colors.Black) previousPegs[i].Color = Colors.Black;
            if (pegList[i].Color == Colors.White) previousPegs[i].Color = Colors.White;
            if (pegList[i].Color == Colors.DarkGray) previousPegs[i].Color = Colors.DarkGray;
        }
    }

    // Play again / Reset functionality
    private void PlayAgain_Clicked(object sender, EventArgs e)
    {
        Button[] btns = { BtnGuess1, BtnGuess2, BtnGuess3, BtnGuess4 };
        BoxView[] pegs = { Peg1, Peg2, Peg3, Peg4 };
        BoxView[] codeSlots = { CodeSlot1, CodeSlot2, CodeSlot3, CodeSlot4 };
        // Hides boards appropriately for start of game
        WinLbl.IsVisible = false;
        LoseLbl.IsVisible = false;
        PlayAgain.IsVisible = false;
        Board.IsVisible = true;
        PegBoard.IsVisible = true;

        // Resets variables
        currentRow = 0;
        blackPegs = 0;
        whitePegs = 0;
        noPegs = 0;
        int[] code = { 0, 0, 0, 0 };
        int[] playerGuess = { 0, 0, 0, 0 };

        // Clears board of older guesses and pegs
        Board.Children.Clear();
        PegBoard.Children.Clear();

        // Adds back the buttons for guessing and reset
        Board.Children.Add(Guess);
        Board.Children.Add(Reset);

        // Adds back the guess buttons and pegs for the first row
        for (int i = 0; i < 4; i++)
        {
            Board.Children.Add(btns[i]);
            PegBoard.Children.Add(pegs[i]);
            btns[i].SetValue(Grid.RowProperty, currentRow);
            pegs[i].SetValue(Grid.RowProperty, currentRow);
            codeSlots[i].IsVisible = false;
        }

        OnStartUp(); // Retriggers OnStartUp to restart game methods

    }


    // File reading and writing - Attempted but not succeeded

    /*public void RebuildBoard(BoxView[] gameRows, int[] playerRows)
    {
        currentRow = 
        for (int i = 0; i < 4; i++)
        {
            Board.Children.Add(gameRows[i]);
            gameRows[i].SetValue(Grid.ColumnProperty, i);
            gameRows[i].SetValue(Grid.RowProperty, _gameState.currentRow);
            gameRows[i].SetValue(BoxView.CornerRadiusProperty, 30);
            gameRows[i].SetValue(BoxView.HeightRequestProperty, 20);
            gameRows[i].SetValue(BoxView.WidthRequestProperty, 20);
            if (playerRows[i] == )
            {
                previousRow[i].Color = Colors.Red;
                colourSaves[i] = 0;
            }
            if (btnList[i].BackgroundColor == Colors.Green)
            {
                previousRow[i].Color = Colors.Green;
                colourSaves[i] = 1;
            }
        }
    }*/

    /*
    GameState ReadJsonFile()
    {
        GameState gs = null;
        string jsonText = "";

        try     // reading the local directory (environment.specialfolders)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fname = Path.Combine(path, filename);
            using (var reader = new StreamReader(fname))
            {
                jsonText = reader.ReadToEnd();
            }
        }
        catch (Exception ex)    // if that fails, then read the embedded resource
        {
            string errorMsg = ex.Message;
        }   // end try catch
        if (jsonText != "")
        {
            gs = new GameState();
            gs = JsonConvert.DeserializeObject<GameState>(jsonText);
            return gs;
        }
        else
            return null;
    }

    private void SaveListOfData(GameState gs)
    {
        string path = Environment.GetFolderPath(
                                       Environment.SpecialFolder.LocalApplicationData);
        string fname = Path.Combine(path, filename);
        using (var writer = new StreamWriter(fname, false))
        {
            string jsonText = JsonConvert.SerializeObject(gs);
            writer.WriteLine(jsonText);
        }
    }

    private void BtnReadFile_Clicked(object sender, EventArgs e)
    {
        string fileContent = "";
        _gameState = ReadJsonFile();

        if (_gameState != null)
        {
            fileContent = "Row values: " + _gameState.playerRows[11] + _gameState.currentRow;
        }
        else
        {
            fileContent = "no file found";
        }
        //        LblFileStuff.Text = fileContent;

    }

    private void BtnWriteFile_Clicked(object sender, EventArgs e)
    {
        GameState gs = new GameState();
        gs.currentRow = currentRow;

        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < currentRow; j++)
            {
                gs.playerRows[i] = (colourSaves[0] + colourSaves[1] + colourSaves[2] + colourSaves[3]);
            }

            SaveListOfData(gs);

        }

    }*/

}

