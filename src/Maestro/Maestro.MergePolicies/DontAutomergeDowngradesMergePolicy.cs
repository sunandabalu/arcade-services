// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Maestro.Contracts;
using System.Threading.Tasks;

namespace Maestro.MergePolicies
{
    public class DontAutomergeDowngradesMergePolicy : MergePolicy
    {
        public override string DisplayName => "Do not automerge downgrades";

        internal static async Task EvaluateDowngradesAsync(IMergePolicyEvaluationContext context)
        {
            IPullRequest pr = context.PullRequest;

            if (await HasAnyDowngradeAsync(pr))
            {
                context.Fail("There are reviews that have requested changes.");
            }
            else
            {
                context.Succeed("No reviews have requested changes.");
            }
        }

        public override async Task EvaluateAsync(IMergePolicyEvaluationContext context, MergePolicyProperties properties)
        {
            await EvaluateDowngradesAsync(context);
        }

        private static async Task<bool> HasAnyDowngradeAsync(IPullRequest pr)
        {
            return await Task.FromResult<bool>(true);
        }
    }
}
