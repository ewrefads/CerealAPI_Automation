    pipeline {

        agent any
        triggers {
          triggers{ pollSCM('H/15 * * * *') }
        }
        stages {

            stage('Checkout') {

                steps {

                    git url: 'https://github.com/ewrefads/CerealAPI_Automation.git', branch: 'master'

                }

            }

            stage('Build and Test') {

                steps {

                    dir('./') {

                        // Check the docker-compose version

                        powershell 'docker-compose --version'

                        // Bring up the services

                        powershell 'docker-compose  -f "docker-compose.yml" -f "docker-compose.override.yml" -f "docker-compose.overideextra.yml" --ansi never up -d'

                        // Ensure the services are running

                        powershell 'docker-compose ps'
                    }

                }

            }

            stage('Deploy') {

                when {

                    expression { currentBuild.result == null || currentBuild.result == 'SUCCESS' }

                }

                steps {

                    echo 'Deploying...'

                    // Add your deploy steps here

                }

            }

        }

        post {

            always {

                echo 'Post actions'

            }

            success {

                echo 'Pipeline completed successfully.'

            }

            failure {

                echo 'Pipeline failed.'

            }

        }

    }

pipeline