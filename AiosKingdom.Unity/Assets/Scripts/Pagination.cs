using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    public Text Indicator;
    public Button Prev;
    public Button Next;

    private int _currentPage = 1;
    public int CurrentPage { get { return _currentPage; } }

    public void Setup(int itemPerPage, int itemCount, Action setItems)
    {
        Prev.onClick.AddListener(() =>
        {
            if (_currentPage - 1 == 1)
            {
                Prev.gameObject.SetActive(false);
            }

            Next.gameObject.SetActive(true);
            --_currentPage;

            setItems();
        });

        Next.onClick.AddListener(() =>
        {
            if ((itemCount - ((_currentPage + 1) * itemPerPage)) <= 0)
            {
                Next.gameObject.SetActive(false);
            }

            Prev.gameObject.SetActive(true);
            ++_currentPage;

            setItems();
        });

        _currentPage = 1;
        Prev.gameObject.SetActive(false);
        Next.gameObject.SetActive(itemCount > itemPerPage);
    }

    public void SetIndicator(int max)
    {
        if (max == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        Indicator.text = string.Format("[{0} / {1}]", _currentPage, max);
    }
}
