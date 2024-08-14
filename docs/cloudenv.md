We want to have a central service that players can connect to so that we can regulate everything from 1 place.

We can make use of cloud bases services like [AWS EC2](https://aws.amazon.com/ec2/).

Install PensionPlan on an AWS EC2 instance.

- Go to the EC2 Dashboard and press Launch instance
- Fill in the name of the EC2 instance
- Select an OS (By preference Linux, so I chose for the 'Ubuntu Server 22.04 LTS (HVM), SSD Volume Type' )

- Create a Keypair
    - Fill in a name, choose RSA and select the .pem private key file format (this is the easiest because it uses OpenSSH).
    - Click Create key pair.
    - It downloads a .pem file, store this somewhere you can find it.

- press Launch instance

To login to the EC2 instance you can do a couple of things but you can find all the options if you select the instance on the left side of your screen with the checkbox, then press connect in the top part of your screen.
Here you have some options on how to connect to the EC2 instance.
```chmod 400``` does not work for windows rather use this command block:
```
# Replace with the path to your .pem file
$filePath = "C:\path\to\file.pem"

# Remove all existing permissions
icacls $filePath /inheritance:r

# Grant read permission to the file owner
icacls $filePath /grant:r "$($env:USERNAME):(R)"

# Optionally, remove permissions for other users/groups if needed
icacls $filePath /remove:g "Users"
icacls $filePath /remove:g "Authenticated Users"
```
Sorry Mac users but you have to look for another solution.

We make use of a docker environment so in our case I logged into the EC2 instance through an ssh command with the .pem file I created earlier.
After that I installed docker on the EC2 instance and copied the docker-compose.yml file from this repository, I ran ```docker compose up```.
(it could be that it says something like it does not have enough permissions, then just put ```sudo``` in front of it).

Now the only thing you have to do is change the security profiles within AWS.
If you go to the instance and click Security you see the inbound and outbound rules, the easiest but unsafest way is to add an inbound rule that accepts all connections.
You could do that for testing purposes but this is not really smart, you'd rather be specific about what connections you want to handle or not.
Like allow tcp traffic on port 5432 (this is the port we use for the database).

For now, I just put all tcp traffic to allowed, and we want to change this in the future.