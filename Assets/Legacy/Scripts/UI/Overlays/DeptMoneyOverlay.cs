using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[Obsolete]
public class DeptMoneyOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text deptAmount;
    [SerializeField] private Transform payLoanOverview;
    [SerializeField] private Transform takeLoanOverview;

    private int dept = 1000;

    public void Start()
    {
        gameObject.SetActive(true);
        deptAmount.text = dept.ToString();
    }

    public void PayLoanOverlay()
    {
        gameObject.SetActive(false);
        payLoanOverview.gameObject.SetActive(true);
        deptAmount.text = dept.ToString();

    }
    public void TakeLoanOverlay()
    {
        gameObject.SetActive(false);
        takeLoanOverview.gameObject.SetActive(true);
    }

}
