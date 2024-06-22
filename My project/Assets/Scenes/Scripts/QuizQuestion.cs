using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public string question; // Câu hỏi
    public string[] answers; // Mảng các đáp án
    public int correctAnswerIndex; // Chỉ số của đáp án đúng

    // Constructor để khởi tạo một đối tượng QuizQuestion
    public QuizQuestion(string question, string[] answers, int correctAnswerIndex)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;
    }
}
