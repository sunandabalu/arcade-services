// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Maestro.Contracts;
using NuGet.Versioning;
using System.Threading.Tasks;

namespace Maestro.MergePolicies
{
    public class DontAutomergeDowngradesMergePolicy : MergePolicy
    {
        public override string DisplayName => "Do not automerge downgrades";

        internal static Task EvaluateDowngradesAsync(IMergePolicyEvaluationContext context)
        {
            IPullRequest pr = context.PullRequest;

            if (HasAnyDowngradeAsync(pr))
            {
                context.Fail("There are reviews that have requested changes.");
            }
            else
            {
                context.Succeed("No reviews have requested changes.");
            }

            return Task.CompletedTask;
        }

        public override async Task EvaluateAsync(IMergePolicyEvaluationContext context, MergePolicyProperties properties)
        {
            await EvaluateDowngradesAsync(context);
        }

        private static bool HasAnyDowngradeAsync(IPullRequest pr)
        {
            foreach (var (update, deps) in pr.RequiredUpdates)
            {
                foreach (var dependency in deps)
                {
                    SemanticVersion.TryParse(dependency.From.Version, out var left);
                    SemanticVersion.TryParse(dependency.To.Version, out var right);

                    if (left.CompareTo(right) < 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
