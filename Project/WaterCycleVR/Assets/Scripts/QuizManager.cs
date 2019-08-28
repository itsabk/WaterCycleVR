using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour {

    public GameObject button1;
    public GameObject button2;


    public Questions[] questions;
    private static List<Questions> unansweredQuestions;

    private Questions currentQuestion;

    [SerializeField]
    private Text factText;

    /*[SerializeField]
    private float delay = 1f;*/

    private int score = 0;
    private int numberOfQuestions;


    void Start()
    {
        if(unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<Questions>();
            numberOfQuestions = unansweredQuestions.Count();
        }

        SetCurrentQuestion();
    }


    void SetCurrentQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count-1);
        currentQuestion = unansweredQuestions[randomQuestionIndex];

        factText.text = currentQuestion.fact;

        unansweredQuestions.RemoveAt(randomQuestionIndex);
    }

   /* IEnumerator TransitionToNextQuestion()
    {
        unansweredQuestions.Remove(currentQuestion);

        yield return new WaitForSeconds(delay);

    }*/

    public void UserSelectTrue()
    {
        if (currentQuestion.isTrue)
        {
            score++;

        }
        qChange();
    }

    public void UserSelectFalse()
    {
        if (!currentQuestion.isTrue)
        {
            score++;
        }
        qChange();
    }

    void qChange()
    {
        if (unansweredQuestions.Count > 0) { SetCurrentQuestion(); }
        else
        {
            button1.SetActive(false);
            button2.SetActive(false);
            factText.text = "Your Score is : " + score.ToString() + " out of " + numberOfQuestions;
        }
    }

}
