using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour {

    public SudokuTile tilePrefab;
    public RectTransform layoutGUI;

    private int[,] sudokuLayout;
    public int numberOfCells = 9;

    public struct CellIndex{
        public int x;
        public int y;

        public CellIndex(int thisX, int thisY){
            x = thisX;
            y = thisY;
        }
    }

    public SudokuTile[,] allTiles;

    public float stepThroughTime = 1.0f;

    private void Start()
    {
        sudokuLayout = new int[numberOfCells, numberOfCells];

        CreateCanvasLayout();

        StartCoroutine("SetLayoutValues");

    }

    private IEnumerator SetLayoutValues()
    {

        for (int y = 0; y < numberOfCells; y++)
        {
            List<int> allAvailableNumbersPerCell = new List<int>();

            // check vertically and have a list of available numbers vert
            List<int> currentRowAvailableNumbers = GetVerticalAvailableNumbers(y);
            allAvailableNumbersPerCell = currentRowAvailableNumbers;

            bool validRow = false;
            while (!validRow)
            {
                // get horizontally available numbers
                for (int x = 0; x < numberOfCells; x++)
                {
                    CellIndex cellIndex = new CellIndex(x, y);
                    allAvailableNumbersPerCell = GetAvailableNumberAtCell(cellIndex);

                    sudokuLayout[x, y] = PickRandomIntFromList(allAvailableNumbersPerCell);

                    DisplayCurrentLayout();
                    HighlightCurrentCell(cellIndex);

                    LogIntList(allAvailableNumbersPerCell, "allAvailableNumbersPerCell");

                    yield return new WaitForSeconds(stepThroughTime);
                }

                validRow = CheckRowForValidity(y);
                if (!validRow)
                {
                    yield return new WaitForSeconds(stepThroughTime);
                    ResetRow(y);
                    yield return new WaitForSeconds(stepThroughTime);
                }
            }        

        }

    }

    bool CheckRowForValidity(int rowIndex){
        bool isValid = true;

        for (int i = 0; i < numberOfCells; i++)
        {
            if(sudokuLayout[i, rowIndex] == 0){
                isValid = false;
            }
        }

        return isValid;
    }

    void ResetRow(int rowIndex){
        for (int i = 0; i < numberOfCells; i++)
        {
            sudokuLayout[i, rowIndex] = 0;
        }
    }

    void CreateCanvasLayout(){

        allTiles = new SudokuTile[numberOfCells, numberOfCells];

        for (int y = 0; y < numberOfCells; y++)
        {
            for (int x = 0; x < numberOfCells; x++)
            {
                SudokuTile newTile = Instantiate(tilePrefab, layoutGUI);
                newTile.SetValue(sudokuLayout[x, y]);
                allTiles[x, y] = newTile;
            }
        }
    }

    void DisplayCurrentLayout(){
        for (int y = 0; y < numberOfCells; y++)
        {
            for (int x = 0; x < numberOfCells; x++)
            {
                allTiles[x, y].SetValue(sudokuLayout[x, y]);
                CellIndex currentCellIndex = new CellIndex(x, y);
            }
        }
    }

    public void HighlightCurrentCell(CellIndex currentCellIndex)
    {
        foreach(SudokuTile tile in allTiles){
            tile.Unhighlight();
        }
        allTiles[currentCellIndex.x, currentCellIndex.y].Highlight();
    }

    public List<int> GetAvailableNumberAtCell(CellIndex cellIndex)
    {
        List<int> allAvailableCellNumbers = new List<int>();
        for (int i = 0; i < numberOfCells; i++)
        {
            allAvailableCellNumbers.Add(i + 1);
        }

        List<int> unavailableNumbersVertical = InvertAvailability(GetVerticalAvailableNumbers(cellIndex.x));
        List<int> unavailableNumbersHorizontal = InvertAvailability(GetHorizontalAvailableNumbers(cellIndex.y));


        for (int unVi = 0; unVi < unavailableNumbersVertical.Count; unVi++)
        {
            if(allAvailableCellNumbers.Contains(unavailableNumbersVertical[unVi])){
                allAvailableCellNumbers.Remove(unavailableNumbersVertical[unVi]);
            }
        }

        for (int unHi = 0; unHi < unavailableNumbersHorizontal.Count; unHi++)
        {
            if (allAvailableCellNumbers.Contains(unavailableNumbersHorizontal[unHi]))
            {
                allAvailableCellNumbers.Remove(unavailableNumbersHorizontal[unHi]);
            }
        }

        List<int> unavailableSquareNumbers = InvertAvailability(GetSquareAvailableNumbers(cellIndex));
        for (int unSi = 0; unSi < unavailableSquareNumbers.Count; unSi++)
        {
            if (allAvailableCellNumbers.Contains(unavailableSquareNumbers[unSi]))
            {
                allAvailableCellNumbers.Remove(unavailableSquareNumbers[unSi]);
            }
        }

        LogIntList(unavailableNumbersVertical, "unavailableNumbersVertical");
        LogIntList(unavailableNumbersHorizontal, "unavailableNumbersHorizontal");
        LogIntList(unavailableSquareNumbers, "unavailableSquareNumbers");


        return allAvailableCellNumbers;
    }

    public List<int> GetVerticalAvailableNumbers(int currentRowIndex){

        List<int> verticalAvailableNumbers = new List<int>();
        for (int i = 0; i < numberOfCells; i++)
        {
            verticalAvailableNumbers.Add(i + 1);
        }

        for (int y = 0; y < numberOfCells; y++)
        {
            int getNumber = sudokuLayout[currentRowIndex, y];
            if (getNumber != 0)
            {
                verticalAvailableNumbers.Remove(getNumber);
            }
        }

        return verticalAvailableNumbers;
    }



    public List<int> GetHorizontalAvailableNumbers(int currentColumnIndex)
    {
        // fill horizontalAvailableNumbers with 1-9
        List<int> horizontalAvailableNumbers = new List<int>();
        for (int i = 0; i < numberOfCells; i++)
        {
            horizontalAvailableNumbers.Add(i + 1);
        }

        // look through horizontal and leave remaining numbers
        for (int x = 0; x < numberOfCells; x++)
        {
            int getNumber = sudokuLayout[x, currentColumnIndex];
            if (getNumber != 0) { 
                horizontalAvailableNumbers.Remove(getNumber);
            }
        }

        return horizontalAvailableNumbers;
    }

    public List<int> GetSquareAvailableNumbers(CellIndex cellIndex)
    {
        List<int> squareAvailableNumbers;
        // get currently used numbers
        List<int> currentSquareNumbers = GetCurrentNumbersFromSquare(cellIndex);
        // invert to get the still available numbers
        squareAvailableNumbers = InvertAvailability(currentSquareNumbers);

        return squareAvailableNumbers;
    }

    public List<int> GetCurrentNumbersFromSquare(CellIndex cellIndex){

        int xSquareIndex = (cellIndex.x - (cellIndex.x % 3)) / 3;
        int ySquareIndex = (cellIndex.y - (cellIndex.y % 3)) / 3;

        int minXIndex = xSquareIndex * 3;
        int maxXIndex = minXIndex + 3;
        int minYIndex = ySquareIndex * 3;
        int maxYIndex = minYIndex + 3;

        List<int> numbersInSquare = new List<int>();

        for (int y = minYIndex; y < maxYIndex; y++)
        {
            for (int x = minXIndex; x < maxXIndex; x++)
            {
                int currentCellValue = sudokuLayout[x, y];
                if(currentCellValue != 0){
                    numbersInSquare.Add(currentCellValue);
                }
            }
        }

        return numbersInSquare;
    }


    // -------
    // UTILITY
    // -------

    public List<int> InvertAvailability(List<int> listToInvert){
        List<int> invertedList = new List<int>();
        for (int i = 0; i < numberOfCells; i++)
        {
            int valueToAdd = i + 1;
            if (!listToInvert.Contains(valueToAdd)){
                invertedList.Add(valueToAdd);
            }
        }

        return invertedList;
    }

    public void LogIntList(List<int> listToLog, string listName = null){

        string stringToLog = listName != null ? listName + " :" : "List: ";

        for (int i = 0; i < listToLog.Count; i++)
        {
            stringToLog += listToLog[i].ToString()+", ";
        }

        Debug.Log(stringToLog);
    }

    public int PickRandomIntFromList(List<int> listToPickFrom){
       
        if(listToPickFrom.Count >= 1){
            int listSize = listToPickFrom.Count;
            int randomIndex = Random.Range(0, listSize);

            return listToPickFrom[randomIndex];
        }
        else{
            return 0;
        }

    }
}