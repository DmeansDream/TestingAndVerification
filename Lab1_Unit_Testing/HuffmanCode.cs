using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HuffmanCode
{
    private string input;

    private SortedDictionary<char, int> occurances;
    private List<KeyValuePair<char, int>> descList;

    private List<Node> nodes = new List<Node>();
    Dictionary<char, string> codeMap = new Dictionary<char, string>();

    public HuffmanCode(string InputMessage)
    {
        this.input = InputMessage;
        char[] chars = input.ToCharArray();

        occurances = chars.Aggregate(new SortedDictionary<char, int>(), (dict, ch) =>
        {
            if (!dict.TryAdd(ch, 1))
            {
                dict[ch]++;
            }

            return dict;
        });

        descList = occurances.OrderBy(k => k.Value).ToList();
    }

    public Node ConstructTree()
    {
        nodes = new List<Node>();

        foreach (var kvp in descList)
        {
            nodes.Add(new Node(kvp.Value, kvp.Key));
        }

        while (nodes.Count > 1)
        {
            Node left = nodes[0];
            Node right = nodes[1];

            Node parent = new Node(left.occurances + right.occurances, '\0', left, right);

            nodes.Remove(left);
            nodes.Remove(right);

            nodes.Add(parent);

            nodes = nodes.OrderBy(n => n.occurances).ToList();
        }

        Node root = nodes[0];
        codeMap = new Dictionary<char, string>();

        PassTheTree(root, "", codeMap);

        foreach (var kvp in codeMap)
        {
            Debug.Log($"Key : {kvp.Key} : Code : {kvp.Value}");
        }

        return root;
    }

    public string PerformEncoding()
    {
        string encodedMessage = String.Concat(input.Select(c => codeMap[c]));

        Debug.Log(encodedMessage);

        Debug.Log($"Original msg size in ASCII: {input.Length * 8 / 8} bytes");
        Debug.Log($"Max msg size in UTF-8: {input.Length * 32 / 8} bytes");
        Debug.Log($"Encoded size: {encodedMessage.Length / 8} bytes*");
        return encodedMessage;
    }
    
    static public void PassTheTree(Node node, string currentCode, Dictionary<char, string> codeMap)
    {
        if (node == null) return;

        Debug.Log($"Node: {node.letter} : Occurs: {node.occurances}");

        if (node.left == null && node.right == null)
        {
            if (node.letter != '\0')
            {
                codeMap.Add(node.letter, currentCode);
                return;
            }
        }

        PassTheTree(node.left, currentCode + "0", codeMap);
        PassTheTree(node.right, currentCode + "1", codeMap);
    }

    static public string PerformDecoding(string msg, Node root)
    {
        StringBuilder decodedMsg = new StringBuilder();
        Node current = root;

        foreach (char bit in msg)
        {
            if (bit == '0')
            {
                current = current.left;
            }
            else if (bit == '1')
            {
                current = current.right;
            }

            if (current.left == null && current.right == null)
            {
                decodedMsg.Append(current.letter);
                current = root;
            }
        }

        return decodedMsg.ToString();
    }

}

public class Node
{
    public char letter;
    public int occurances;
    public Node left;
    public Node right;

    public Node(int times = 0, char let = default, Node left = null, Node right = null)
    {
        letter = let;
        occurances = times;
        this.left = left;
        this.right = right;
    }
}
