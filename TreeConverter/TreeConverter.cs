using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeConverter
{
    public static class TreeConverter
    {
        static Node currentNode;

        public static void ConvertGrammarTreeToOperationTree(ref Node root)
        {
            currentNode = root;

        step1:
            if (!currentNode.HasNonTerminals())
                return;

         step2:
            currentNode = currentNode.GetLeftmostNonTerminalChild();

        step3:
            if (currentNode.HasOnlyOneChild())
            {
                RetargetNode(currentNode);
                goto step1;
            }
            else goto step4;

        step4:
            if (currentNode.HasTerminalChildWithSemanticlessTerminal(out var semanticlessNode))
            {
                currentNode.Children.Remove(semanticlessNode);
                goto step1;
            }
            else goto step5;

        step5:
            if (currentNode.HasOperationChild(out var operationChildNode))
            {
                // todo Здесь может быть проблема, потому что безусловно считаем, что если тут операция, то все остальные символы - операнды
                currentNode.Data = operationChildNode.Data;
                currentNode.Children.Remove(operationChildNode);
                goto step6;
            }

        step6:
            if (currentNode.HasNonTerminals())
            {
                currentNode = currentNode.GetLeftmostNonTerminalChild();
                goto step3;
            }
            else return;
        }

        /// <summary>
        /// Изменяет ссылки на потомков и родителей для текущего узла
        /// </summary>
        /// <param name="node"></param>
        private static void RetargetNode(Node node)
        {
            if (node.Children.Count > 1)
            {
                throw new Exception("Node contains more than one children. Operation is inconsistent!");
            }

            foreach (Node node1 in node.Children)
            {
                node1.Parent = node;
            }
            node.Children = node.Children[0].Children;
        }

    }
}
