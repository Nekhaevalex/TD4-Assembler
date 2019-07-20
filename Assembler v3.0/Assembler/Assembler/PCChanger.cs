using AST;
namespace Opcode
{
    internal interface PCChanger
    {
        ASTNode Link { get; set; }
    }
}