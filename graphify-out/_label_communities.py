import json
from pathlib import Path

analysis = json.loads(Path("graphify-out/.graphify_analysis.json").read_text(encoding="utf-8"))
extraction = json.loads(Path("graphify-out/.graphify_extract.json").read_text(encoding="utf-8"))

# Build node lookup
node_map = {n["id"]: n for n in extraction["nodes"]}

# Show communities by size
communities = analysis["communities"]
by_size = sorted(communities.items(), key=lambda kv: len(kv[1]), reverse=True)

print("Top 20 communities by node count:")
for cid, members in by_size[:20]:
    # Sample node labels
    labels = []
    for m in members[:5]:
        if m in node_map:
            labels.append(node_map[m].get("label", m))
    print("  Community %s: %d nodes (e.g. %s)" % (cid, len(members), ", ".join(labels[:3])))
