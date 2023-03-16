namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Registry : StatefulApp {
    public new const string Name = "registry";
    public const string Image = "registry:2";
    public const int Port = 5000;

    public const string PvcName = "registry-data";
    public const string MountPath = "/var/lib/registry";

    public const string LoadBalancerName = "registry-external";

    public readonly Output<string>? LoadBalancerIP;

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public Registry(Namespace ns,
        PvcTemplateVolume pvc,
        string? ingressHost = null,
        int? lbPort = null,
        string name = Name
    ) : base(ns, name, "api", Port, Image,
        new Volume[] {
            pvc,
        }
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}