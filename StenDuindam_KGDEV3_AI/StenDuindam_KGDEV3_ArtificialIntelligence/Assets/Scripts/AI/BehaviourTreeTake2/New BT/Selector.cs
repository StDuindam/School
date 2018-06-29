using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : BTNode {

    private BTNode[] nodes;

    public Selector(params BTNode[] nodes) {
        this.nodes = nodes;
        }

    public override ReturnType Run() {

        foreach (BTNode node in nodes) {
            ReturnType result = node.Run();
            switch (result) {
                case ReturnType.Success:
                    Debug.Log("Success!");
                    return ReturnType.Success;
                case ReturnType.Failure:
                    break;
                case ReturnType.Running:
                    break;
                }
            }


        return ReturnType.Failure;
        }
    }
