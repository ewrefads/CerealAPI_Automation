    pipeline {

        agent any

        stages {

            stage('Checkout') {

                steps {

                    git url: 'https://github.com/ewrefads/CerealAPI_Automation.git', branch: 'master'

                }

            }

            stage('Build and Test') {

                steps {

                    dir('CerealAPI') {

                        // Check the docker-compose version

                        sh 'docker-compose --version'

                        // Bring up the services

                        sh 'docker-compose up -d'

                        // Ensure the services are running

                        sh 'docker-compose ps'
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