﻿// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.UnionAll.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbUnionAllExpression expression)
        {
            Expression left = this.Visit(expression.Left);
            Expression right = this.Visit(expression.Right);

            Type resultType = edmTypeConverter.Convert(expression.ResultType).GetElementType();
            Type rightType = TypeHelper.GetElementType(right.Type);


            ParameterExpression param = Expression.Parameter(rightType);

            List<MemberBinding> bindings = new List<MemberBinding>();

            PropertyInfo[] sourceProps = rightType.GetProperties();
            PropertyInfo[] resultProps = resultType.GetProperties();

            List<Expression> initializers = new List<Expression>();

            for (int i = 0; i < sourceProps.Length; i++)
            {
                initializers.Add(Expression.Property(param, sourceProps[i]));
            }

            Expression body = Expression.New(resultType.GetConstructors().Single(), initializers, resultType.GetProperties());
            right = queryMethodExpressionBuilder.Select(right, Expression.Lambda(body, param));

            return queryMethodExpressionBuilder.Concat(left, right);
        }
    }
}