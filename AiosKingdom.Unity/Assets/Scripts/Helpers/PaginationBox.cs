using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PaginationBox : MonoBehaviour
{
    [Header("Pagination")]
    public GameObject List;
    public GameObject Box;
    public int ItemPerPage = 5;

    public GameObject ListItemPrefab;
    public GameObject PaginationPrefab;

    protected Pagination _pagination;
}
