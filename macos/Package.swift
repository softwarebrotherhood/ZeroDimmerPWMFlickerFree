// swift-tools-version:5.7
import PackageDescription

let package = Package(
    name: "ZeroDimmer",
    platforms: [
        .macOS(.v12)
    ],
    products: [
        .executable(name: "ZeroDimmer", targets: ["ZeroDimmer"])
    ],
    targets: [
        .executableTarget(
            name: "ZeroDimmer",
            path: "Sources"
        )
    ]
)
