﻿using CppSharp.AST;
using CppSharp.Generators;

namespace MonoEmbeddinator4000.Generators
{
    public class JavaSources : CTemplate
    {
        public JavaSources(BindingContext context, Options options,
            TranslationUnit unit)
         : base(context, options, unit)
        {
        }

        public override string FileExtension => "java";

        public string AssemblyId => CGenerator.AssemblyId(Unit);

        public override void Process()
        {
            GenerateFilePreamble();

            PushBlock();
            PopBlock(NewLineKind.BeforeNextBlock);

            VisitDeclContext(Unit);
        }

        public override void WriteHeaders()
        {
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            return true;
        }

        public override bool VisitClassDecl(Class @class)
        {
            VisitDeclContext(@class);

            return true;
        }
    }
}