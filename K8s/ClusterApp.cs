namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;

using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;
using Service = global::Pulumi.Kubernetes.Core.V1.Service;
using Ingress = global::Pulumi.Kubernetes.Networking.V1.Ingress;

public class ClusterApp {
    public readonly Namespace Namespace;
    public readonly string Name;
    public readonly Deployment Deployment;
    public readonly Service Service;
    public readonly Output<string> IP;

    public ClusterApp(Namespace ns, string name, string image, string portName, int portNumber) {
        Namespace = ns;
        Name = name;
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber)
        });
        Deployment = K8s.Deployment(ns, name, container).Apply(name);
        Service = K8s.Service(Deployment, new[] {
            K8s.ServicePort(portName, portNumber, portNumber),
        }).Apply(name);
        IP = Service.ClusterIP();
    }

    public ClusterApp(Namespace ns, string name, string image, string portName1, int portNumber1, string portName2, int portNumber2) {
        Namespace = ns;
        Name = name;
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
        });
        Deployment = K8s.Deployment(ns, name, container).Apply(name);
        Service = K8s.Service(Deployment, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
        }).Apply(name);
        IP = Service.ClusterIP();
    }

    public ClusterApp(Namespace ns, string name, string image, string portName1, int portNumber1, string portName2, int portNumber3, string portName3, int portNumber2) {
        Namespace = ns;
        Name = name;
        var container = K8s.Container(name, image, new[] {
            K8s.ContainerPort(portNumber1),
            K8s.ContainerPort(portNumber2),
            K8s.ContainerPort(portNumber3),
        });
        Deployment = K8s.Deployment(ns, name, container).Apply(name);
        Service = K8s.Service(Deployment, new[] {
            K8s.ServicePort(portName1, portNumber1, portNumber1),
            K8s.ServicePort(portName2, portNumber2, portNumber2),
            K8s.ServicePort(portName3, portNumber3, portNumber3),
        }).Apply(name);
        IP = Service.ClusterIP();
    }

    public Ingress ApplyHostIngress(string ingressHost, int servicePort) {
        return K8s.Ingress(Namespace, ingressHost, K8s.IngressSpec(new [] {
            K8s.IngressRule(ingressHost, K8s.HTTPIngressRuleValue(new [] {
                K8s.HTTPIngressPath(K8s.IngressBackend(Name, servicePort))
            })),
        })).Apply(ingressHost);
    }
}

public partial class K8s {
    public static InputMap<string> AppLabels(string name) {
        return new InputMap<string> {
            { "app", name },
        };
    }

    public static ObjectMetaArgs AppMeta(Namespace ns, string name) => new ObjectMetaArgs {
        Namespace = ns.Value,
        Name = name,
        Labels = AppLabels(name),
    };
}