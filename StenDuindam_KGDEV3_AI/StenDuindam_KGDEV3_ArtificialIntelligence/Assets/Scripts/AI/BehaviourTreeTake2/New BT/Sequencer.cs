using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sequencer : BTNode {

    private BTNode[] nodes;

    public Sequencer(params BTNode[] nodes) {
        this.nodes = nodes;
    }

    public override ReturnType Run() {

        foreach(BTNode node in nodes) {
            ReturnType result = node.Run();
            switch (result) {
                case ReturnType.Success:
                    break;
                case ReturnType.Failure:
                    return ReturnType.Failure;
                case ReturnType.Running:
                    return ReturnType.Running;
            }
        }

        return ReturnType.Success;

    }
}
