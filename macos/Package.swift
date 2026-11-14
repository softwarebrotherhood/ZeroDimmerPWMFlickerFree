// swift-tools-version:5.7
import PackageDescription

let package = Package(
    name: "FlickerFreeDimmer",
    platforms: [
        .macOS(.v12)
    ],
    products: [
        .executable(name: "FlickerFreeDimmer", targets: ["FlickerFreeDimmer"])
    ],
    targets: [
        .executableTarget(
            name: "FlickerFreeDimmer",
            path: "Sources"
        )
    ]
)
