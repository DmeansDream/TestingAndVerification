using NUnit.Framework;

public class HuffmanCodeTesting
{
    [Test]
    [Explicit, Category("Huffman")]
    public void HuffmanCodeEncodingTest()
    {
        // Arrange
        string msgToEncode = "This is a test encoding message.";
        var encoderUnderTest = new HuffmanCode(msgToEncode);
        string expected =
            "100111011000011111000011111000111101000010111100011001001111010000101010100001110110110101110101111110011011001010010";
        
        // Act
        encoderUnderTest.ConstructTree();
        string result = encoderUnderTest.PerformEncoding();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [Explicit, Category("Huffman")]
    public void HuffmanCodeDecodingTest()
    {
        // Arrange
        string msgToEncode = "This is a test encoding message.";
        var encoderUnderTest = new HuffmanCode(msgToEncode);
        
        // Act
        Node root = encoderUnderTest.ConstructTree();
        string encodedMsg = encoderUnderTest.PerformEncoding();
        string result = HuffmanCode.PerformDecoding(encodedMsg, root);
        
        // Assert
        Assert.That(result, Is.EqualTo(msgToEncode));
    }
    
}
